using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Newtonsoft.Json.Linq;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class DeleteAccountCommand: MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute DeleteAccountCommand");

            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message.Chat.Id);

            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("Cancel", Culture), replyMarkup: Keyboards.StartKeyboard);

                return;
            }

            if (userState == null)
            {
                var accountsKeyboard = (await RequestClient.Instance.GetUserAccountsAsync(message.Chat.Id))?.ToNameKeyboard();

                userState = new State
                {
                    StateName = "select_account_to_delete",
                    StateObject = new DeleteAccountDto()
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("SelectAccountToDelete", Culture), parseMode: ParseMode.Html, replyMarkup: accountsKeyboard);
            }
            else if (userState.StateName == "select_account_to_delete")
            {
                userState.StateName = "input_account_password";
                userState.StateObject!.AccountUserName = message.Text;

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("InputAccountPassword", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState.StateName == "input_account_password")
            {
                userState.StateObject!.AccountPassword = message.Text;
                userState.StateObject!.TelegramId = message.Chat.Id;

                var deleteSuccess = await RequestClient.Instance.DeleteAccountAsync((userState.StateObject as JObject)!.ToObject<DeleteAccountDto>()!);

                if (deleteSuccess)
                {
                    await StateMachine.RemoveStateAsync(message.Chat.Id);
                    await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("AccountDeletedSuccessfully", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
                }
                else
                {
                    var accountsKeyboard = (await RequestClient.Instance.GetUserAccountsAsync(message.Chat.Id))?.ToNameKeyboard();

                    userState.StateName = "select_account_to_delete";
                    await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                    await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("SelectAccountToDelete", Culture), parseMode: ParseMode.Html, replyMarkup: accountsKeyboard);
                }
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("DeleteAccount", Culture), "select_account_to_delete", "input_account_password"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
