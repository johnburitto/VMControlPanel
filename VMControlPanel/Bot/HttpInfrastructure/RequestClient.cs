using Bot.StateMachineBase;
using Core.Dtos;
using Core.Entities;
using Newtonsoft.Json;
using System.Text;
using UserInfrastructure.Service.Interfaces;

namespace Bot.HttpInfrastructure
{
    public static class RequestClient
    {
        private static HttpClient? _client;
        private static readonly object _lock = new();

        public static HttpClient? Client => GetInstance();

        private static HttpClient? GetInstance()
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

        public static async Task<List<User>?> GetUserAccountsAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8080/api/Auth/accounts/{telegramId}");
            
            return JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> GetStateAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetStateNameAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
            var obj = JsonConvert.DeserializeObject<State>(await response.Content.ReadAsStringAsync());

            return obj?.StateName ?? "";
        }

        public static async Task SaveStateAsync(long telegramId, State state, float expTimeInHours)
        {
            var stateSting = JsonConvert.SerializeObject(state);

            await Client!.PostAsync($"https://localhost:8081/api/Cache/{telegramId}_state?value={stateSting}&expTimeInHours={expTimeInHours}", null);
        }

        public static async Task RemoveStateAsync(long telegramId)
        {
            await Client!.DeleteAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
        }

        public static async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8080/api/Auth/login", content);

            return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
