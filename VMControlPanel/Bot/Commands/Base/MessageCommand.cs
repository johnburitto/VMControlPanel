using Bot.StateMachineBase;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Commands.Base
{
    public class MessageCommand : Command
    {
        public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var state = await StateMachine.GetSateAsync(message!.Chat.Id);
            var data = state == null ? message.Text : state.StateName;

            if (IsCanBeExecuted(data ?? ""))
            {
                await ExecuteAsync(client, message);
            }
        }

        public override Task TryExecuteAsync(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            Console.WriteLine("This is message command");

            return Task.CompletedTask;
        }
    }
}
