using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.Utilities;
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
            var accounts = await RequestClient.GetUserAccountsAsync(message!.Chat.Id);

            Console.WriteLine(LocalizationManager.GetString("HelloMessage"));

            await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("HelloMessage")}\n\n{accounts?.ToStringList()}", 
                parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
        }
    }
}
