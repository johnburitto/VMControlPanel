using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserInfrastructure.Service.Interfaces;

namespace Bot.Commands
{
    public class AuthCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "❌ Відмінити" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "/auth", "Увійти в акаунт", "input_username", "input_password" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);
            
            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);

                return;
            }

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_username",
                    StateObject = new LoginDto()
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть ім'я користувача:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_username")
            {
                userState.StateObject!.UserName = message?.Text;
                userState.StateName = "input_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть пароль:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else
            {
                userState!.StateObject!.Password = message?.Text;
                userState!.StateObject!.TelegramId = message?.Chat.Id;

                var response = await RequestClient.LoginAsync((userState.StateObject as JObject)!.ToObject<LoginDto>()!);

                if (response == AuthResponse.SuccessesLogin)
                {
                    var virtualMachines = await RequestClient.GetUserVirtualMachinesAsync(message!.Chat.Id);

                    await client.SendTextMessageAsync(message!.Chat.Id, "Ви успішно увійшли до системи", parseMode: ParseMode.Html, replyMarkup: virtualMachines.ToKeyboard());
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
                else if (response == AuthResponse.BadCredentials)
                {
                    userState.StateName = "input_username";

                    await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                    await client.SendTextMessageAsync(message!.Chat.Id, "Введіть ім'я користувача:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
                }
            }
        }
    }
}
