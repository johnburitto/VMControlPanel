using Bot.Commands.Base;
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
    public class RegisterCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "❌ Відмінити" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "/register", "Створити акаунт", "create_username", "create_password", "create_email" ];

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
                    StateName = "create_username",
                    StateObject = new RegisterDto()
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Придумайте ім'я користувача:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState!.StateName == "create_username")
            {
                userState.StateObject!.UserName = message?.Text;
                userState.StateName = "create_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Придумайте пароль:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState!.StateName == "create_password")
            {
                userState.StateObject!.Password = message?.Text;
                userState.StateName = "create_email";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть адрес електронної пошти:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else
            {
                userState.StateObject!.Email = message?.Text;
                userState.StateObject!.TelegramId = message?.Chat.Id;

                var response = await RequestClient.RegisterAsync((userState.StateObject as JObject)!.ToObject<RegisterDto>()!);

                if (response == AuthResponse.SuccessesRegister)
                {
                    await client.SendTextMessageAsync(message!.Chat.Id, "Ви успішно зареєструвалися", parseMode: ParseMode.Html);
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
                else if (response == AuthResponse.AlreadyRegistered)
                {
                    await client.SendTextMessageAsync(message!.Chat.Id, "Користувач з даним іменем вже зареєстрований", parseMode: ParseMode.Html);
                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
            }
        }
    }
}
