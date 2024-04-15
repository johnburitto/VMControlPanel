using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ExitCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "🚪 Вийти із акаунта", "/exit" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var dto = new SSHRequestDto
            {
                VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message!.Chat.Id}_vm"),
                UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync()
            };

            await StateMachine.RemoveStateAsync(message.Chat.Id);
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_vm");
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_current_user_id");
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_auth");
            await RequestClient.DisposeClientAndStream(dto);
            await client.SendTextMessageAsync(message.Chat.Id, "Ви вийшли із системи", parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
        }
    }
}
