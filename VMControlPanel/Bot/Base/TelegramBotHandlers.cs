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
        private readonly RequestClient _client;
        private readonly List<MessageCommand> _commands;
        
        public TelegramBotHandlers()
        {
            _commands = new List<MessageCommand>() {
                new StartCommand(_client),
                new ExitCommand(_client),
                new AuthCommand(_client),
                new RegisterCommand(_client),
                new AddVirtualMachineCommand(_client),
                new ChooseVirtualMachineCommand(_client),
                new ExecuteSSHCommandsCommand(_client),
                new CreateDirectoryCommand(_client),
                new DeleteDirectoryCommand(_client),
                new GetFileFromVirtualMachineCommand(_client),
                new UploadFileToVirtualMachineCommand(_client),
                new GetMetricsCommand(_client),
                new ChangeLanguageCommand(_client)
            };
        }

        public TelegramBotHandlers(RequestClient client)
        {
            _client = client;
            _commands = new List<MessageCommand>() {
                new StartCommand(_client),
                new ExitCommand(_client),
                new AuthCommand(_client),
                new RegisterCommand(_client),
                new AddVirtualMachineCommand(_client),
                new ChooseVirtualMachineCommand(_client),
                new ExecuteSSHCommandsCommand(_client),
                new CreateDirectoryCommand(_client),
                new DeleteDirectoryCommand(_client),
                new GetFileFromVirtualMachineCommand(_client),
                new UploadFileToVirtualMachineCommand(_client),
                new GetMetricsCommand(_client),
                new ChangeLanguageCommand(_client)
            };
        }


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
                await command.TryExecuteAsync(client, message);
            }
        }

        private async Task CallbackQueryHandlerAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            foreach (var command in _commands)
            {
                await command.TryExecuteAsync(client, callbackQuery);
            }
        }
    }
}
