using Newtonsoft.Json;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class CacheRequestClientExtensions
    {
        public static async Task CacheAsync(this RequestClient client, string key, string value, float expTimeInHours)
        {
            await client.Client!.PostAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{key}?value={value}&expTimeInHours={expTimeInHours}", null);
        }

        public static async Task<T?> GetCachedAsync<T>(this RequestClient client, string key)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{key}");

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> GetCachedAsync(this RequestClient client, string key)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{key}");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task DeleteCachedAsync(this RequestClient client, string key)
        {
            var response = await client.Client!.DeleteAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{key}");
        }

        public static async Task<bool> CheckIfHasCacheAsync(this RequestClient client, string key)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{key}");
            var data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            return data != null;
        }
    }
}
