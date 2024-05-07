﻿using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.StateMachineBase;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class ExecuteSSHCommandsCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute ExecuteSSHCommandsCommand");

            Keyboards.Culture = Culture;

            if (message!.Text!.Contains('❌'))
            {
                await StateMachine.RemoveStateAsync(message!.Chat.Id);
                await client.SendTextMessageAsync(message.Chat.Id, LocalizationManager.GetString("Cancel", Culture), replyMarkup: Keyboards.VMActionKeyboard);

                return;
            }

            var userState = await StateMachine.GetSateAsync(message!.Chat.Id);

            if (userState == null)
            {
                userState = new State
                {
                    StateName = "input_command",
                    StateObject = null
                };

                await StateMachine.SaveStateAsync(message.Chat.Id, userState);
                await client.SendTextMessageAsync(message.Chat.Id, $"{LocalizationManager.GetString("InputCommand", Culture)}:", parseMode: ParseMode.Html);
            }
            else if (userState.StateName == "input_command")
            {
                var dto = new SSHRequestDto
                {
                    VirtualMachine = await RequestClient.Instance.GetCachedAsync<VirtualMachine>($"{message.Chat.Id}_vm"),
                    Command = message.Text,
                    UserId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message.Chat.Id}_current_user_id")).Content.ReadAsStringAsync(),
                    TelegramId = message.Chat.Id
                };
                var response = await RequestClient.Instance.ExecuteSSHCommandAsync(dto);

                await client.SendTextMessageAsync(message.Chat.Id, $"```\n{response}\n```", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.CancelKeyboard);
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("ExecuteCommands", Culture), "input_command"];

            return base.TryExecuteAsync(client, message);
        }
    }
}