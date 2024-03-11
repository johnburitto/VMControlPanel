using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class AuthCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "/auth", "input_username", "input_password" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);
        
            if (userState == null)
            {
                var dto = new LoginDto();
                
                userState = new State
                {
                    StateName = "input_username",
                    StateObject = dto
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть ім'я користувача:", parseMode: ParseMode.Html);
            }
            else if (userState?.StateName == "input_username")
            {
                userState.StateObject!.UserName = message?.Text;
                userState.StateName = "input_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть пароль:", parseMode: ParseMode.Html);
            }
            else
            {
                userState!.StateObject!.Password = message?.Text;

                var response = await RequestClient.LoginAsync((userState.StateObject as JObject)!.ToObject<LoginDto>()!);

                await client.SendTextMessageAsync(message!.Chat.Id, response, parseMode: ParseMode.Html);
            }
        }
    }
}
