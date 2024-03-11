using Bot.StateMachineBase;
using Core.Entities;
using Newtonsoft.Json;

namespace Bot.HttpInfrastructure
{
    public static class RequestClient
    {
        private static HttpClient? _client;
        private static object _lock = new object();

        public static HttpClient? Client => GetInstance();

        private static HttpClient? GetInstance()
        {
            if (_client == null)
            {
                lock (_lock)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri("https://localhost:8080");
                }
            }

            return _client;
        }

        public static async Task<List<User>?> GetUserAccountsAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"/api/Auth/accounts/{telegramId}");
            
            return JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> GetStateAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"/api/Cache/{telegramId}_state");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task SaveStateAsync(long telegramId, State state, long expTimeInHours)
        {
            var stateSting = JsonConvert.SerializeObject(state);
            var content = new StringContent(stateSting);

            await Client!.PostAsync($"/api/Cache/{telegramId}_state?expTimeInHours={expTimeInHours}", content);
        }
    }
}
