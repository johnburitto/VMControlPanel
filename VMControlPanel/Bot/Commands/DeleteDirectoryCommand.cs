using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Core.Entities;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Utilities;
using Bot.Localization;
using Bot.HttpInfrastructure.Extensions;

namespace Bot.Commands
{
    public class DeleteDirectoryCommand : MessageCommand
    {
        public DeleteDirectoryCommand(RequestClient requestClient) : base(requestClient)
        {

        }

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_delete_directory_name"
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputDirectoryName", Culture)}:", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.Null);
            }
            else if (userState.StateName! == "input_delete_directory_name")
            {
                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await _requestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Data = message.Text,
                    UserId = await (await _requestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                    TelegramId = message.Chat.Id
                };

                var response = await _requestClient.DeleteDirectoryAsync(dto);

                await StateMachine.RemoveStateAsync(message.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.VMActionKeyboard);
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("DeleteDirectory", Culture), "input_delete_directory_name"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
