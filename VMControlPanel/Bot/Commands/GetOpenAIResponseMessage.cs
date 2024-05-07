using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetOpenAIResponseMessage : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "/ask" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var response = await RequestClient.Instance.GetOpenAIResponse(message!.Text!.Replace("/ask", ""));

            await client.SendTextMessageAsync(message.Chat.Id, response, parseMode: ParseMode.MarkdownV2);
        }
    }
}
