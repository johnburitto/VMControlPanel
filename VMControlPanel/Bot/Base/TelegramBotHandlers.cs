using Bot.Commands;
using Bot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Base
{
    public class TelegramBotHandlers : ITelegramBotHandlers
    {
        private readonly List<MessageCommand> _messageCommands =
            [
                new StartCommand()
            ];
        private readonly List<CallbackQueryCommand> _callbackQueryCommands = [];

        public async Task MessagesHandlerAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        await MessageHandlerAsync(client, update.Message);
                    } return;
                case UpdateType.CallbackQuery:
                    {
                        await CallbackQueryHandlerAsync(client, update.CallbackQuery);
                    } return;
                default: return;
            }
        }

        public Task ErrorHandlerAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram Bot API exception:\n {apiRequestException.ErrorCode}\n {apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);

            return Task.CompletedTask;
        }

        private async Task MessageHandlerAsync(ITelegramBotClient client, Message? message)
        {
            foreach (var command in _messageCommands)
            {
                await command.TryExecuteAsync(client, message);
            }
        }

        private async Task CallbackQueryHandlerAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            foreach (var command in _callbackQueryCommands)
            {
                await command.TryExecuteAsync(client, callbackQuery);
            }
        }
    }
}
