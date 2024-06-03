using Azure.AI.OpenAI;
using Bot.Configurations;
using Microsoft.Extensions.Options;

namespace Bot.HttpInfrastructure
{
    public class RequestClient
    {
        private HttpClient? _client;
        private OpenAIClient? _openAIClient;
        private static RequestClient? _instance;
        private static readonly object _lock = new();

        public static RequestClient Instance => GetInstance();
        public HttpClient Client => GetClientInstance();
        public OpenAIClient OpenAIClient => GetOpenAIClient();
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

        private HttpClient GetClientInstance()
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

        private OpenAIClient GetOpenAIClient()
        {
            if (_openAIClient == null)
            {
                lock (_lock)
                {
                    _openAIClient = new OpenAIClient(Instance.ApiConfiguration!.OpenAIKey);
                }
            }

            return _openAIClient;
        }

        public static void Configure(IOptions<ApiConfiguration>? configuration)
        {
            Instance.ApiConfiguration = configuration?.Value;
        }
    }
}
