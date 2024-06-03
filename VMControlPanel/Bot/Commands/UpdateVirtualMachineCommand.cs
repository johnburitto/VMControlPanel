using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class UpdateVirtualMachineCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute UpdateVirtualMachineCommand");

            Keyboards.Culture = Culture;

            var userState = await StateMachine.GetSateAsync(message.Chat.Id);

            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("Cancel", Culture), replyMarkup: Keyboards.VMActionKeyboard);

                return;
            }

            if (userState == null) 
            {
                userState = new State()
                {
                    StateName = "update_vm_name",
                    StateObject = new VirtualMachineDto()
                }; await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputVMName", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "update_vm_name")
            {
                userState.StateName = "update_vm_username";
                userState.StateObject!.Name = message.Text;

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputVMUser", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "update_vm_username")
            {
                userState.StateName = "update_vm_password";
                userState.StateObject!.UserName = message.Text;

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputVMUserPassword", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "update_vm_password")
            {
                userState.StateName = "update_vm_host";
                userState.StateObject!.Password = message.Text;

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputVMHost", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "update_vm_host")
            {
                userState.StateName = "update_vm_port";
                userState.StateObject!.Host = message.Text;

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("InputVMPort", Culture)}:", parseMode: ParseMode.Html, replyMarkup: Keyboards.CancelKeyboard);
            }
            else if (userState?.StateName == "update_vm_port")
            {
                try
                {
                    userState.StateObject!.Port = int.Parse(message.Text);
                    userState.StateObject!.UserId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message!.Chat.Id}_current_user_id"))
                        .Content.ReadAsStringAsync();
                    userState.StateObject!.TelegramId = message.Chat.Id;

                    var virtualMachine = await RequestClient.Instance.UpdateVirtualMachineAsync((userState.StateObject as JObject)!.ToObject<VirtualMachineDto>()!);

                    if (virtualMachine != null)
                    {
                        await RequestClient.Instance.CacheAsync($"{message!.Chat.Id}_vm", JsonConvert.SerializeObject(virtualMachine), 1f);
                        await client.SendTextMessageAsync(message!.Chat.Id, LocalizationManager.GetString("SuccessesUpdate", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);

                        Log.Information($"[{userState.StateObject.UserId}] virtual machine successfully updated");
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message!.Chat.Id, LocalizationManager.GetString("UpdattingError", Culture), parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);

                        Log.Information($"[{userState.StateObject.UserId}] some error has occured when virtual machine was updating");
                    }

                    await StateMachine.RemoveStateAsync(message!.Chat.Id);
                }
                catch (Exception)
                {
                    userState.StateName = "input_vm_port";

                    await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                }
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("UpdateVirtualmachine", Culture), "update_vm_name", "update_vm_username", "update_vm_password", "update_vm_host", "update_vm_port"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
