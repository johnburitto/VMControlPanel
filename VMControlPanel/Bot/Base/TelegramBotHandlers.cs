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
        private readonly List<MessageCommand> _commands =
            [
                new StartCommand(),
                new ExitCommand(),
                new AuthCommand(),
                new RegisterCommand(),
                new AddVirtualMachineCommand(),
                new ChooseVirtualMachineCommand(),
                new ExecuteSSHCommandsCommand(),
                new CreateDirectoryCommand(),
                new DeleteDirectoryCommand(),
                new GetFileFromVirtualMachineCommand(),
                new UploadFileToVirtualMachineCommand(),
                new GetMetricsCommand(),
                new ChangeLanguageCommand(),
                new GetOpenAIResponseMessage()
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
            foreach (var command in _commands)
            {
                if (await command.TryExecuteAsync(client, message))
                {
                    return;
                }
            }
        }

        private async Task CallbackQueryHandlerAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            foreach (var command in _commands)
            {
                if (await command.TryExecuteAsync(client, callbackQuery))
                {
                    return;
                }
            }
        }
    }
}
