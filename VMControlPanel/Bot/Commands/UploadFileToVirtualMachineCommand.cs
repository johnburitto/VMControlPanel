using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class UploadFileToVirtualMachineCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute UploadFileToVirtualMachineCommand");

            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_upload_file",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputFile", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.Null);
            }
            else if (userState.StateName == "input_upload_file")
            {
                if (message.Document == null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputFile", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.Null);

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
                    VirtualMachine = await RequestClient.Instance.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    FilePath = $"{FileManager.FileDirectory}/{message.Document.FileName}",
                    UserId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                    TelegramId = message.Chat.Id
                };
                var response = await RequestClient.Instance.UploadFileToVirtualMachine(dto);

                await client.SendTextMessageAsync(message.Chat.Id, response, parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
                await StateMachine.RemoveStateAsync(message.Chat.Id);
                FileManager.DeleteFile($"{FileManager.FileDirectory}/{message.Document.FileName}");
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("UploadFile", Culture), "input_upload_file"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
