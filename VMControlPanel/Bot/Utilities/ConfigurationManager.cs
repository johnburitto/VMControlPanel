using Bot.Configurations;
using Bot.HttpInfrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utilities;

namespace Bot.Utilities
{
    public static class ConfigurationManager
    {
        private static IConfiguration? _configuration;
        private static object _lock = new object();

        public static IConfiguration Configuration => GetConfiguration();

        private static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                lock (_lock)
                {
                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                }
            }

            return _configuration;
        }

        public static void ConfigureApp()
        {
            var services = new ServiceCollection();

            services.Configure<ApiConfiguration>(options => Configuration.GetSection("ApiConfiguration").Bind(options));

            var serviceProvider = services.BuildServiceProvider();

            RequestClient.Configure(serviceProvider.GetService<IOptions<ApiConfiguration>>());
            CryptoService.BlowfishKey = Configuration["BlowfishKey"]!;
        }
    }
}
