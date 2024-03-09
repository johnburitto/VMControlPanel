using Bot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class StartCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "/start" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            await client.SendTextMessageAsync(message!.Chat.Id, "Привіт! Я допоможу тобі взаємодіяти із твоїми віртуальними машинами", parseMode: ParseMode.Html);
        }
    }
}
