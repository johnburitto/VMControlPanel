using Bot.Base;
using Bot.Configurations;
using Bot.HttpInfrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .SetBasePath (Directory.GetCurrentDirectory ())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

services.AddOptions<ApiConfiguration>().Bind(configuration.GetSection("ApiConfiguration"));
services.AddSingleton<RequestClient>();

var bot = new TelegramBot<TelegramBotHandlers>();

bot.Init();
bot.StartReceiving();
