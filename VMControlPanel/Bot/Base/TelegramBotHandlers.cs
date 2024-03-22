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
        private readonly List<MessageCommand> _commands =
            [
                new StartCommand(),
                new AuthCommand(),
                new RegisterCommand(),
                new AddVirtualMachineCommand(),
                new ChooseVirtualMachineCommand()
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
            if (await StateHandlerAsync(client, message!.Chat.Id, message))
            {
                return;
            }

            foreach (var command in _commands)
            {
                await command.TryExecuteAsync(client, message);
            }
        }

        private async Task CallbackQueryHandlerAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            if (await StateHandlerAsync(client, callbackQuery!.Message!.Chat.Id, callbackQuery))
            {
                return;
            }

            foreach (var command in _commands)
            {
                await command.TryExecuteAsync(client, callbackQuery);
            }
        }

        private async Task<bool> StateHandlerAsync(ITelegramBotClient client, long telegramId, dynamic data)
        {
            var state = await RequestClient.GetStateNameAsync(telegramId);

            foreach (var command in _commands)
            {
                if (command.IsCanBeExecuted(state))
                {
                    await command.ExecuteAsync(client, data);

                    return true;
                }
            }

            return false;
        } 
    }
}
