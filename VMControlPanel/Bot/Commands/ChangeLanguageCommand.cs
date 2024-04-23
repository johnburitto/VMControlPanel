using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ChangeLanguageCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "choose_language",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("ChooseLanguage", Culture), parseMode: ParseMode.Html, replyMarkup: LocalizationManager.GetLanguagesKeyboard());
            }
            else if (userState.StateName == "choose_language")
            {
                var language = LocalizationManager.DetermineLanguage(message.Text);

                if (language == null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("ChooseLanguage", Culture), parseMode: ParseMode.Html, replyMarkup: LocalizationManager.GetLanguagesKeyboard());
                }
                else
                {
                    Culture = (Cultures)language;
                    Keyboards.Culture = Culture;
                    NoAuthCommands.Culture = Culture;

                    await RequestClient.CacheAsync($"{message.Chat.Id}_culture", ((int)language).ToString(), 1f);
                    await StateMachine.RemoveStateAsync(message.Chat.Id);
                    await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("SuccessesChange", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
                }
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("ChangeLanguage", Culture), ..LocalizationManager.GetLanguages(), "choose_language"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
