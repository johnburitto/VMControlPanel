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
            if (IsCanBeExecuted(callbackQuery?.Data ?? ""))
            {
                await ExecuteAsync(client, callbackQuery);
            }
        }
    }
}
