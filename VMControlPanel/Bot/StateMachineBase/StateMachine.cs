using Bot.HttpInfrastructure;
using Newtonsoft.Json;

namespace Bot.StateMachineBase
{
    public static class StateMachine
    {
        public static long ExpTimeInHours { get; set; } = 1;

        public static async Task<State?> GetSateAsync(long telegramId)
        {
            var stateSting = await RequestClient.GetStateAsync(telegramId);

            return JsonConvert.DeserializeObject<State>(stateSting);
        }

        public static async Task SaveStateAsync(long telegramId, State state)
        {
            await RequestClient.SaveStateAsync(telegramId, state, telegramId);
        }
    }
}
