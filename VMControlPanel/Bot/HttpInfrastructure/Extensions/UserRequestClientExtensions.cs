using Core.Dtos;
using Core.Entities;
using Newtonsoft.Json;
using System.Text;
using UserInfrastructure.Service.Interfaces;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class UserRequestClientExtensions
    {
        public static async Task<List<User>?> GetUserAccountsAsync(this RequestClient client, long telegramId)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.UserApiUrl}/Auth/accounts/{telegramId}");

            return JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<AuthResponse> LoginAsync(this RequestClient client, LoginDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.UserApiUrl}/Auth/login", content);

            return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<AuthResponse> RegisterAsync(this RequestClient client, RegisterDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.UserApiUrl}/Auth/register", content);

            return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
