using Bot.StateMachineBase;
using Newtonsoft.Json;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class StateRequestClientExtensions
    {
        public static async Task<string> GetStateAsync(this RequestClient client, long telegramId)
        {
            var response = await client.Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetStateNameAsync(this RequestClient client, long telegramId)
        {
            var response = await client.Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
            var obj = JsonConvert.DeserializeObject<State>(await response.Content.ReadAsStringAsync());

            return obj?.StateName ?? "";
        }

        public static async Task SaveStateAsync(this RequestClient client, long telegramId, State state, float expTimeInHours)
        {
            var stateSting = JsonConvert.SerializeObject(state);

            await client.Client!.PostAsync($"https://localhost:8081/api/Cache/{telegramId}_state?value={stateSting}&expTimeInHours={expTimeInHours}", null);
        }

        public static async Task RemoveStateAsync(this RequestClient client, long telegramId)
        {
            await client.Client!.DeleteAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
        }
    }
}
