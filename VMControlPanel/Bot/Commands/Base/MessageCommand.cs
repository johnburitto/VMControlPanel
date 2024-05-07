using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

            if (token.IsNullOrEmpty() && !NoAuthCommands.Commands.Contains(message.Text!) && state == null)
            {
                state = new State
                {
                    StateName = "start_auth",
                    StateObject = new LoginDto()
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, state);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("YouHaveToLogin"), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);

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
