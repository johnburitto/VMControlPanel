using Bot.Commands;
using Bot.Commands.Base;
using Bot.HttpInfrastructure;
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
        private readonly List<Command> _stateCommands =
            [
                new AuthCommand()
            ];

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
            var state = await RequestClient.GetStateNameAsync(message!.Chat.Id);

            foreach (var command in _stateCommands)
            {
                if (command.IsCanBeExecuted(state) || command.IsCanBeExecuted(message.Text!))
                {
                    await command.ExecuteAsync(client, message);

                    return;
                }
            }

            foreach (var command in _messageCommands)
            {
                await command.TryExecuteAsync(client, message);
            }
        }

        private async Task CallbackQueryHandlerAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            var state = await RequestClient.GetStateNameAsync(callbackQuery!.Message!.Chat.Id);

            foreach (var command in _stateCommands)
            {
                if (command.IsCanBeExecuted(state) || command.IsCanBeExecuted(callbackQuery.Data!))
                {
                    await command.ExecuteAsync(client, callbackQuery);

                    return;
                }
            }

            foreach (var command in _callbackQueryCommands)
            {
                await command.TryExecuteAsync(client, callbackQuery);
            }
        }
    }
}
