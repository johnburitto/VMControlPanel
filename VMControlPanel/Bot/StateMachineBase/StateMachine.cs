using Bot.HttpInfrastructure;
using Newtonsoft.Json;

namespace Bot.StateMachineBase
{
    public static class StateMachine
    {
        public static float ExpTimeInHours { get; set; } = 1f;
        public static RequestClient RequestClient = new RequestClient();

        public static async Task<State?> GetSateAsync(long telegramId)
        {
            var stateSting = await RequestClient.GetStateAsync(telegramId);

            return JsonConvert.DeserializeObject<State>(stateSting);
        }

        public static async Task SaveStateAsync(long telegramId, State state)
        {
            await RequestClient.SaveStateAsync(telegramId, state, ExpTimeInHours);
        }

        public static async Task RemoveStateAsync(long telegramId)
        {
            await RequestClient.RemoveStateAsync(telegramId);
        }
    }
}
