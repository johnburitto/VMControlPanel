using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public abstract class Command
    {
        public virtual List<string>? Names { get; set; }

        public virtual Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task ExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            throw new NotImplementedException();
        }

        public virtual Task TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsCanBeExecuted(string message)
        {
            foreach (var name in Names ?? [])
            {
                if (message.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
