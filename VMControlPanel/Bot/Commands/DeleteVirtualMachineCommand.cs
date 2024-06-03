using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class DeleteVirtualMachineCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute DeleteVirtualMachineCommand");

            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "are_you_shure"
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("AreYouShure", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.AreYouShureKeyboard);
            }
            else if (userState.StateName == "are_you_shure")
            {
                if (message.Text == LocalizationManager.GetString("Yes", Culture))
                {
                    var deleteSuccess = await RequestClient.Instance.DeleteVirtualMachineAsync(message.Chat.Id);

                    if (deleteSuccess)
                    {
                        var dto = new SSHRequestDto
                        {
                            VirtualMachine = await RequestClient.Instance.GetCachedAsync<VirtualMachine>($"{message!.Chat.Id}_vm"),
                            UserId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync()
                        };
                        var virtualMachinesKeybard = (await RequestClient.Instance.GetUserVirtualMachinesAsync(message.Chat.Id)).ToKeyboard(Culture);

                        await RequestClient.Instance.DisposeClientAndStream(dto);
                        await RequestClient.Instance.DeleteCachedAsync($"{message.Chat.Id}_vm");
                        await StateMachine.RemoveStateAsync(message.Chat.Id);
                        await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("VirtualMachineDeletedSuccessfully", Culture), parseMode: ParseMode.Html, replyMarkup: virtualMachinesKeybard);
                    }
                }
                else
                {
                    await StateMachine.RemoveStateAsync(message.Chat.Id);
                    await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("Cancel", Culture), replyMarkup: Keyboards.VMActionKeyboard);
                }
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("DeleteVirtualMachine", Culture), "are_you_shure"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
