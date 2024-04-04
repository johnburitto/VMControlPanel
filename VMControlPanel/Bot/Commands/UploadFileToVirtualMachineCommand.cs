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
    public class UploadFileToVirtualMachineCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "Виконувати команди" },
            new KeyboardButton[] { "Створити директорію", "Видалити директорію" },
            new KeyboardButton[] { "Завантажити файл", "Вивантажити файл" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "Вивантажити файл", "input_upload_file" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_upload_file",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, "Надішліть файл, який хочете вивантажити:", parseMode: ParseMode.Html);
            }
            else if (userState.StateName == "input_upload_file")
            {
                if (message.Document == null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Надішліть файл, який хочете вивантажити:", parseMode: ParseMode.Html);

                    return;
                }
                else
                {
                    FileManager.CreateDirectory();

                    using (var fileStream = System.IO.File.Create($"{FileManager.FileDirectory}/{message.Document.FileName}"))
                    {
                        await client.GetInfoAndDownloadFileAsync(message.Document.FileId, fileStream);
                    }
                }

                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    FilePath = $"{FileManager.FileDirectory}/{message.Document.FileName}",
                    UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync()
                };
                var response = await RequestClient.UploadFileToVirtualMachine(dto);

                await client.SendTextMessageAsync(message.Chat.Id, response, parseMode: ParseMode.Html, replyMarkup: _keyboard);
                await StateMachine.RemoveStateAsync(message.Chat.Id);
                FileManager.DeleteFile($"{FileManager.FileDirectory}/{message.Document.FileName}");
            }
        }
    }
}
