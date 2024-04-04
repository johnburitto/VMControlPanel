using Azure;
using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class GetFileFromVirtualMachineCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "Виконувати команди" },
            new KeyboardButton[] { "Створити директорію", "Видалити директорію" },
            new KeyboardButton[] { "Завантажити файл", "Вивантажити файл" }
        ])
        {
            ResizeKeyboard = true
        };
        
        public override List<string>? Names { get; set; } = ["Завантажити файл", "input_download_file_name"];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_download_file_name",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, "Введіть шлях до файлу:", parseMode: ParseMode.Html);
            }
            else if (userState.StateName == "input_download_file_name")
            {
                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Data = message.Text,
                    UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync()
                };

                var response = await RequestClient.GetFileFromVirtualMachineAsync(dto);

                if (response!.IsUploaded)
                {
                    using (var fileStream = FileManager.OpenFileAsStream(response.FilePath!))
                    {
                        await client.SendDocumentAsync(message.Chat.Id, document: InputFile.FromStream(fileStream, response.FilePath!.Split("/").Last()),
                            caption: response.Message, parseMode: ParseMode.Html);
                    }
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, response.Message!, parseMode: ParseMode.Html, replyMarkup: _keyboard);
                }

                FileManager.DeleteFile(response.FilePath!);
                await StateMachine.RemoveStateAsync(message.Chat.Id);
            }
        }
    }
}
