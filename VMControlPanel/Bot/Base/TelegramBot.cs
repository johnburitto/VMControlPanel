using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Bot.Base
{
    public class TelegramBot<THandlers> where THandlers : ITelegramBotHandlers, new()
    {
        protected TelegramBotClient? Bot {  get; set; }
        protected ReceiverOptions? ReceiverOptions { get; set; }
        protected string Token => "7184645442:AAEPDLlEIPjN6FD_9ZV_himaW8dh3gCEtoI";
        protected CancellationTokenSource CancellationTokenSource => new();
        
        protected THandlers Handlers = new();
    
        public void Init()
        {
            Bot = new TelegramBotClient(Token);
            ReceiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [],
                ThrowPendingUpdates = true
            };

            Log.Information("Bot initialized");
        }

        public void StartReceiving()
        {
            Bot?.StartReceiving(
                Handlers.MessagesHandlerAsync,
                Handlers.ErrorHandlerAsync,
                ReceiverOptions,
                CancellationTokenSource.Token);

            Log.Information("Bot start receiving");
            Console.ReadKey();
            CancellationTokenSource.Cancel();
        }
    }
}
