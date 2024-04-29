using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class StartCommand : MessageCommand
    {
        public StartCommand(RequestClient requestClient) : base(requestClient)
        {

        }

        public override List<string>? Names { get; set; } = [ "/start" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Culture = LocalizationManager.GetLanguage(message!.From!.LanguageCode);
            Keyboards.Culture = Culture;
            NoAuthCommands.Culture = Culture;
            await _requestClient.CacheAsync($"{message!.Chat.Id}_culture", ((int)Culture).ToString(), 1f);

            var accounts = await _requestClient.GetUserAccountsAsync(message!.Chat.Id);

            await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("HelloMessage", Culture)}\n\n{accounts?.ToStringList(Culture)}", 
                parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
        }
    }
}
