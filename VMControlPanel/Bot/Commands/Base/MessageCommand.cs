using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands.Base
{
    public class MessageCommand : Command
    {
        public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var token = await RequestClient.GetCachedAsync($"{message!.Chat.Id}_auth");
            var state = await StateMachine.GetSateAsync(message.Chat.Id);
            var data = state == null ? message.Text : state.StateName;

            if (token.IsNullOrEmpty() && !NoAuthCommands.Commands.Contains(message.Text!) && state == null)
            {
                state = new State
                {
                    StateName = "start_auth",
                    StateObject = new LoginDto()
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, state);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("YouHaveToLogin"), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);

                return;
            }

            if (IsCanBeExecuted(data ?? ""))
            {
                await ExecuteAsync(client, message);
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            Console.WriteLine("This is message command");

            return Task.CompletedTask;
        }
    }
}
