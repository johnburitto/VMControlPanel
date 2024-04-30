using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Newtonsoft.Json;

namespace Bot.StateMachineBase
{
    public static class StateMachine
    {
        public static float ExpTimeInHours { get; set; } = 1f;

        public static async Task<State?> GetSateAsync(long telegramId)
        {
            var stateSting = await RequestClient.Instance.GetStateAsync(telegramId);

            return JsonConvert.DeserializeObject<State>(stateSting);
        }

        public static async Task SaveStateAsync(long telegramId, State state)
        {
            await RequestClient.Instance.SaveStateAsync(telegramId, state, ExpTimeInHours);
        }

        public static async Task RemoveStateAsync(long telegramId)
        {
            await RequestClient.Instance.RemoveStateAsync(telegramId);
        }
    }
}
