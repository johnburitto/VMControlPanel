using Bot.Base;
using Bot.Utilities;

ConfigurationManager.ConfigureApp();

var bot = new TelegramBot<TelegramBotHandlers>();

bot.Init();
bot.StartReceiving();
