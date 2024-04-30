using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.Utilities;
using Newtonsoft.Json;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ChooseVirtualMachineCommand : MessageCommand
    {
        public override List<string>? Names { get; set; }

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute ChooseVirtualMachineCommand");

            Keyboards.Culture = Culture;

            var virtualMachine = await RequestClient.Instance.GetVirtualMachineByUserIdAndVMNameAsync(message!.Chat.Id, message?.Text);
            var userId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message!.Chat.Id}_current_user_id")).Content.ReadAsStringAsync();

            await RequestClient.Instance.CacheAsync($"{message!.Chat.Id}_vm", JsonConvert.SerializeObject(virtualMachine), 1f);
            await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("VMChosen", Culture)} {virtualMachine?.Name}", parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);

            Log.Information($"[{userId}] choosed to work {virtualMachine?.Name}({virtualMachine?.Id})");
        }

        public override async Task TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = await RequestClient.Instance.GetUserVirtualMachinesNamesAsync(message!.Chat.Id);

            await base.TryExecuteAsync(client, message);
        }
    }
}
