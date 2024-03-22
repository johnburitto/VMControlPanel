using Azure;
using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.StateMachineBase;
using Core.Dtos;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ExecuteSSHCommandsCommand : MessageCommand
    {
        public override List<string>? Names { get; set; } = [ "/execute", "Виконати команду", "input_command" ];

        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            if (message!.Text!.Contains("/execute"))
            {
                var dto = new SSHRequestDto
                {
                    VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Command = message.Text.Replace("/execute ", "")
                };
                var response = await RequestClient.ExecuteSSHCommandAsync(dto);

                await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2);
            }
            else
            {
                var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

                if (userState == null)
                {
                    userState = new State
                    {
                        StateName = "input_command",
                        StateObject = null
                    };

                    await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                    await client.SendTextMessageAsync(message.Chat.Id, $"Введіть команду:", parseMode: ParseMode.Html);
                }
                else if (userState.StateName == "input_command")
                {
                    var dto = new SSHRequestDto
                    {
                        VirtualMachine = await RequestClient.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                        Command = message.Text
                    };
                    var response = await RequestClient.ExecuteSSHCommandAsync(dto);

                    await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2);
                }
            }
        }
    }
}