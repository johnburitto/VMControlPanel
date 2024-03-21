using Azure.Core;
using Bot.Commands.Base;
using Bot.Extensions;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Commands
{
    public class AddVirtualMachineCommand : MessageCommand
    {
        private readonly ReplyKeyboardMarkup _keyboard = new([
            new KeyboardButton[] { "❌ Відмінити" }
        ])
        {
            ResizeKeyboard = true
        };

        public override List<string>? Names { get; set; } = [ "➕ Додати нову машину", "input_vm_name", "input_vm_username", 
                                                              "input_vm_password", "input_vm_host", "input_vm_port" ];



        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);

                return;
            }

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_vm_name",
                    StateObject = new VirtualMachineDto()
                };

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть ім'я віртуальної машини:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_vm_name")
            {
                userState.StateObject!.Name = message.Text;
                userState.StateName = "input_vm_username";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть ім'я користувача на віртуальній машині:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_vm_username")
            {
                userState.StateObject!.UserName = message.Text;
                userState.StateName = "input_vm_password";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть пароль користувача на віртуальній машині:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_vm_password")
            {
                userState.StateObject!.Password = message.Text;
                userState.StateName = "input_vm_host";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть хоста віртуальної машини:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_vm_host")
            {
                userState.StateObject!.Host = message.Text;
                userState.StateName = "input_vm_port";

                await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                await client.SendTextMessageAsync(message!.Chat.Id, "Введіть порт віртуальної машини:", parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
            else if (userState?.StateName == "input_vm_port")
            {
                try
                {
                    userState.StateObject!.Port = int.Parse(message.Text);
                    userState.StateObject!.UserId = await (await RequestClient.Client!.GetAsync($"https://localhost:8081/api/Cache/{message!.Chat.Id}_current_user_id"))
                        .Content.ReadAsStringAsync();

                    var virtualMachine = await RequestClient.AddVirtualMachineAsync((userState.StateObject as JObject)!.ToObject<VirtualMachineDto>()!);
                    var virtualMachines = await RequestClient.GetUserVirtualMachinesAsync(message!.Chat.Id);

                    if (virtualMachine != null)
                    {
                        await client.SendTextMessageAsync(message!.Chat.Id, "Віртуальна машина успішно додана", parseMode: ParseMode.Html, replyMarkup: virtualMachines.ToKeyboard()); 
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message!.Chat.Id, "Підчас додавання машини сталася помилка", parseMode: ParseMode.Html, replyMarkup: virtualMachines.ToKeyboard());
                    }

                    await RequestClient.RemoveStateAsync(message!.Chat.Id);
                }
                catch (Exception)
                {
                    userState.StateName = "input_vm_port";

                    await StateMachine.SaveStateAsync(message!.Chat.Id, userState);
                }
            }
        }
    }
}
