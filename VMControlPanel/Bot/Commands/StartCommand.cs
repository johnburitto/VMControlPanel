using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UserInfrastructure.Extensions;

namespace Bot.Commands
{
    public class StartCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "/start" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var accounts = await RequestClient.GetUserAccountsAsync(message!.Chat.Id);

            await client.SendTextMessageAsync(message!.Chat.Id, $"Привіт! Я допоможу тобі взаємодіяти із твоїми віртуальними машинами\n\n{accounts?.ToStringList()}", 
                parseMode: ParseMode.Html);
        }
    }
}
