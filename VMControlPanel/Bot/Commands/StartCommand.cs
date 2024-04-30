using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.Utilities;
using Serilog;
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
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute StartCommand");

            Culture = LocalizationManager.GetLanguage(message!.From!.LanguageCode);
            Keyboards.Culture = Culture;
            NoAuthCommands.Culture = Culture;
            await RequestClient.Instance.CacheAsync($"{message!.Chat.Id}_culture", ((int)Culture).ToString(), 1f);

            var accounts = await RequestClient.Instance.GetUserAccountsAsync(message!.Chat.Id);

            await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("HelloMessage", Culture)}\n\n{accounts?.ToStringList(Culture)}", 
                parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);
        }
    }
}
