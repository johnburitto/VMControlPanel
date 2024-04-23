using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class CreateDirectoryCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ LocalizationManager.GetString("CreateDirectory"), "input_create_directory_name" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_create_directory_name"
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputDirectoryName")}:", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.Null);
            }
            else if (userState.StateName! == "input_create_directory_name")
            {
                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Data = message.Text,
                    UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                    TelegramId = message.Chat.Id
                };

                var response = await RequestClient.CreateDirectoryAsync(dto);

                await StateMachine.RemoveStateAsync(message.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.VMActionKeyboard);
            }
        }
    }
}
