using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserInfrastructure.Extensions;

namespace Bot.Commands
{
    public class StartCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new ReplyKeyboardMarkup([
            new KeyboardButton[] { "Створити акаунт", "Увійти в акаунт" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "/start" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var accounts = await RequestClient.GetUserAccountsAsync(message!.Chat.Id);

            await client.SendTextMessageAsync(message!.Chat.Id, $"Привіт! Я допоможу тобі взаємодіяти із твоїми віртуальними машинами\n\n{accounts?.ToStringList()}", 
                parseMode: ParseMode.Html, replyMarkup: _keyboard);
        }
    }
}
