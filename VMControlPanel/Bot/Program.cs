using Bot.Base;
using Bot.Configurations;
using Bot.HttpInfrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

services.Configure<ApiConfiguration>(options => configuration.GetSection("ApiConfiguration").Bind(options));

var serviceProvider = services.BuildServiceProvider();

RequestClient.Configure(serviceProvider.GetService<IOptions<ApiConfiguration>>());

var bot = new TelegramBot<TelegramBotHandlers>();

bot.Init();
bot.StartReceiving();
