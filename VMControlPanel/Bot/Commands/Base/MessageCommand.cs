using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public class MessageCommand : Command
    {

        public override async Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var culture = await RequestClient.Instance.GetCachedAsync($"{message!.Chat.Id}_culture");

            Culture = culture.IsNullOrEmpty() ? Cultures.En : JsonConvert.DeserializeObject<Cultures>(culture);
            NoAuthCommands.Culture = Culture;

            var token = await RequestClient.Instance.GetCachedAsync($"{message!.Chat.Id}_auth");
            var state = await StateMachine.GetSateAsync(message.Chat.Id);
            var data = state == null ? message.Text : state.StateName;

            if (await ChechAuth(client, message, token, state))
            {
                return false;
            }

            if (IsCanBeExecuted(data ?? ""))
            {
                await ExecuteAsync(client, message);

                return true;
            }

            return false;
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            Console.WriteLine("This is message command");

            return Task.FromResult(false);
        }
    }
}
