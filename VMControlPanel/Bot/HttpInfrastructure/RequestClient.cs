using Bot.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bot.HttpInfrastructure
{
    public class RequestClient
    {
        private HttpClient? _client;
        private static RequestClient? _instance;
        private static readonly object _lock = new();

        public static RequestClient Instance => GetInstance();
        public HttpClient? Client => GetClientInstance();
        public ApiConfiguration? ApiConfiguration { get; set; }

        private static RequestClient GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = new RequestClient();
                }
            }

            return _instance;
        }

        private HttpClient? GetClientInstance()
        {
            if (_client == null)
            {
                lock (_lock)
                {
                    _client = new HttpClient();
                }
            }

            return _client;
        }

        public static void Configure(IOptions<ApiConfiguration>? configuration)
        {
            Instance.ApiConfiguration = configuration?.Value;
        }
    }
}
