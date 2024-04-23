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
    public class ExitCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Keyboards.Culture = Culture;

            var dto = new SSHRequestDto
            {
                VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message!.Chat.Id}_vm"),
                UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync()
            };

            await StateMachine.RemoveStateAsync(message.Chat.Id);
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_vm");
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_current_user_id");
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_auth");
            await RequestClient.DeleteCachedAsync($"{message.Chat.Id}_culture");
            await RequestClient.DisposeClientAndStream(dto);
            await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("LoggedOut", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [$"🚪 {LocalizationManager.GetString("Logout", Culture)}", "/exit"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
