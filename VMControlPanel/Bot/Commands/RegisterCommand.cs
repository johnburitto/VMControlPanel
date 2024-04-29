using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UserInfrastructure.Service.Interfaces;

namespace Bot.Commands
{
    public class RegisterCommand : MessageCommand
    {
        public RegisterCommand(RequestClient requestClient) : base(requestClient)
        {

        }

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("Cancel", Culture), replyMarkup: Keyboards.StartKeyboard);

                return;
            }

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "create_username",
                    StateObject = new RegisterDto()
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputUserName", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState!.StateName == "create_username")
            {
                userState.StateObject!.UserName = message?.Text;
                userState.StateName = "create_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputPassword", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState!.StateName == "create_password")
            {
                userState.StateObject!.Password = message?.Text;
                userState.StateName = "create_email";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputEmail", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else
            {
                userState.StateObject!.Email = message?.Text;
                userState.StateObject!.TelegramId = message?.Chat.Id;
                userState.StateObject!.Culture = LocalizationManager.GetLanguage(message!.From!.LanguageCode);

                var response = await RequestClient.RegisterAsync((userState.StateObject as JObject)!.ToObject<RegisterDto>()!);

                if (response == AuthResponse.SuccessesRegister)
                {
                    var virtualMachines = await RequestClient.GetUserVirtualMachinesAsync(message!.Chat.Id);

                    await client.SendTextMessageAsync(message!.Chat.Id, LocalizationManager.GetString("SuccessesRegister", Culture), parseMode: ParseMode.Html, replyMarkup: virtualMachines.ToKeyboard(Culture));
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
                else if (response == AuthResponse.AlreadyRegistered)
                {
                    await client.SendTextMessageAsync(message!.Chat.Id, LocalizationManager.GetString("AlreadyRegistered", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = ["/register", LocalizationManager.GetString("Register", Culture), "create_username", "create_password", "create_email"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
