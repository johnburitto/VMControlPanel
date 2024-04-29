using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.Utilities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ChooseVirtualMachineCommand : MessageCommand
    {
        public ChooseVirtualMachineCommand(RequestClient requestClient) : base(requestClient)
        {

        }

        public override List<string>? Names { get; set; }

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Keyboards.Culture = Culture;

            var virtualMachine = await _requestClient.GetVirtualMachineByUserIdAndVMNameAsync(message!.Chat.Id, message?.Text);

            await _requestClient.CacheAsync($"{message!.Chat.Id}_vm", JsonConvert.SerializeObject(virtualMachine), 1f);
            await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("VMChosen", Culture)} {virtualMachine?.Name}", parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
        }

        public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = await _requestClient.GetUserVirtualMachinesNamesAsync(message!.Chat.Id);

            await base.TryExecuteAsync(client, message);
        }
    }
}
