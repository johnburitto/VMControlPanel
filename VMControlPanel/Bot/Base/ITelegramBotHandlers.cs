using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Base
{
    public interface ITelegramBotHandlers
    {
        Task MessagesHandlerAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken);
        Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
    }
}
