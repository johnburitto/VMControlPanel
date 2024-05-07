using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public class CallbackQueryCommand : Command
    {
        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Console.WriteLine("This is callback query command");

            return Task.FromResult(false);
        }

        public override async Task<bool> TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            var state = await StateMachine.GetSateAsync(callbackQuery!.Message!.Chat.Id);
            var data = state == null ? callbackQuery.Data : state.StateName;

            if (IsCanBeExecuted(data ?? ""))
            {
                await ExecuteAsync(client, callbackQuery);

                return true;
            }

            return false;
        }
    }
}
