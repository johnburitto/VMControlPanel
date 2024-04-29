using Bot.HttpInfrastructure;
using Core.Entities;
using Org.BouncyCastle.Asn1.Ocsp;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public abstract class Command
    {
        public virtual List<string>? Names { get; set; }
        protected static Cultures Culture { get; set; } = Cultures.En;
        protected readonly RequestClient _requestClient;

        public Command(RequestClient requestClient)
        {
            _requestClient = requestClient;
        }

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
                if (message.ToLower().Contains(name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
