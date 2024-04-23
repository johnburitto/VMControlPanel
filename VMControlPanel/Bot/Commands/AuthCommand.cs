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
    public class AuthCommand : MessageCommand
    {
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

            if (userState == null || userState?.StateName == "start_auth")
            {
                userState = new State
                {
                    StateName = "input_username",
                    StateObject = new LoginDto()
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("LoginUserName", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "input_username")
            {
                userState.StateObject!.UserName = message?.Text;
                userState.StateName = "input_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("LoginPassword", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else
            {
                userState!.StateObject!.Password = message?.Text;
                userState!.StateObject!.TelegramId = message?.Chat.Id;

                var response = await RequestClient.LoginAsync((userState.StateObject as JObject)!.ToObject<LoginDto>()!);

                if (response == AuthResponse.SuccessesLogin)
                {
                    var virtualMachines = await RequestClient.GetUserVirtualMachinesAsync(message!.Chat.Id);

                    await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("SuccessesLogin", Culture)}", parseMode: ParseMode.Html, replyMarkup: virtualMachines.ToKeyboard(Culture));
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
                else if (response == AuthResponse.BadCredentials)
                {
                    userState.StateName = "input_username";

                    await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                    await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("LoginUserName", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
                }
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = ["/auth", LocalizationManager.GetString("Login", Culture), "start_auth", "input_username", "input_password"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
