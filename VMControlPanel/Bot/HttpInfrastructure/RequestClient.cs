using Bot.Configurations;
using Microsoft.Extensions.Options;

namespace Bot.HttpInfrastructure
{
    public class RequestClient
    {
        public readonly ApiConfiguration _apiConfiguration;
        private HttpClient? _client;
        //private static RequestClient? _instance;
        private static readonly object _lock = new();

        public HttpClient? Client => GetClientInstance();
        //public static RequestClient Instance => GetInstance();

        //private static RequestClient GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        lock (_lock)
        //        {
        //            _instance = new RequestClient();
        //        }
        //    }

        //    return _instance;
        //}

        public RequestClient()
        {

        }

        public RequestClient(IOptions<ApiConfiguration> apiConfiguration)
        {
            _apiConfiguration = apiConfiguration.Value;
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
    }
}
