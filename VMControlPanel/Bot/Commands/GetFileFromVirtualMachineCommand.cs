using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetFileFromVirtualMachineCommand : MessageCommand
    {        
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_download_file_name",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputFilePath", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.Null);
            }
            else if (userState.StateName == "input_download_file_name")
            {
                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Data = message.Text,
                    UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                    TelegramId = message.Chat.Id
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
                    await client.SendTextMessageAsync(message.Chat.Id, response.Message!, parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
                }

                FileManager.DeleteFile(response.FilePath!);
                await StateMachine.RemoveStateAsync(message.Chat.Id);
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("DownloadFile", Culture), "input_download_file_name"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
