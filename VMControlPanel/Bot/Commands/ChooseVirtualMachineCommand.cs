using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class ChooseVirtualMachineCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "Виконувати команди" },
            new KeyboardButton[] { "Створити директорію", "Видалити директорію" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; }

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var virtualMachine = await RequestClient.GetVirtualMachineByUserIdAndVMNameAsync(message!.Chat.Id, message?.Text);

            await RequestClient.CacheAsync($"{message!.Chat.Id}_vm", JsonConvert.SerializeObject(virtualMachine), 1f);
            await client.SendTextMessageAsync(message!.Chat.Id, $"Ви обрали для взаємодії віртуальну машину {virtualMachine?.Name}", parseMode: ParseMode.Html, replyMarkup: _keyboard);
        }

        public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = await RequestClient.GetUserVirtualMachinesNamesAsync(message!.Chat.Id);

            await base.TryExecuteAsync(client, message);
        }
    }
}
