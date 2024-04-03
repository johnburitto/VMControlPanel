using Azure;
using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class CreateDirectoryCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "Виконувати команди" },
            new KeyboardButton[] { "Створити директорію", "Видалити директорію" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "Створити директорію", "input_create_directory_name" ];

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
                await client.SendTextMessageAsync(message.Chat.Id, $"Введіть назву директорії:", parseMode: ParseMode.MarkdownV2, replyMarkup: null);
            }
            else if (userState.StateName! == "input_create_directory_name")
            {
                var dto = new SFTPRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Data = message.Text,
                    UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                };

                var response = await RequestClient.CreateDirectoryAsync(dto);

                await StateMachine.RemoveStateAsync(message.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2, replyMarkup: _keyboard);
            }
        }
    }
}
