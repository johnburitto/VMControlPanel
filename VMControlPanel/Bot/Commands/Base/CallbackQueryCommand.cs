using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public class CallbackQueryCommand : Command
    {
        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Console.WriteLine("This is callback query command");

            return Task.CompletedTask;
        }

        public override async Task TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            var state = await StateMachine.GetSateAsync(callbackQuery!.Message!.Chat.Id);
            var data = state == null ? callbackQuery.Data : state.StateName;

            if (IsCanBeExecuted(data ?? ""))
            {
                await ExecuteAsync(client, callbackQuery);
            }
        }
    }
}
