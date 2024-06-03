using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands.Base
{
    public abstract class Command
    {
        public virtual List<string>? Names { get; set; }
        protected static Cultures Culture { get; set; } = Cultures.En;

        public virtual Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task ExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsCanBeExecuted(string message)
        {
            foreach (var name in Names ?? [])
            {
                if (message.ToLower().Contains(name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        protected async Task<bool> ChechAuth(ITelegramBotClient client, Message message, string token, State? state)
        {
            if (token.IsNullOrEmpty() && !NoAuthCommands.Commands.Contains(message.Text!) && state == null)
            {
                state = new State
                {
                    StateName = "start_auth",
                    StateObject = new LoginDto()
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, state);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("YouHaveToLogin"), parseMode: ParseMode.Html, replyMarkup: Keyboards.StartKeyboard);

                return true;
            }

            return false;
        }
    }
}
