﻿using Bot.Commands.Base;
using Bot.HttpInfrastructure;
using Bot.HttpInfrastructure.Extensions;
using Bot.Localization;
using Bot.Utilities;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Newtonsoft.Json;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Commands
{
    public class GetMetricsCommand : MessageCommand
    {
        public override async Task ExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Log.Information($"[{message!.Chat.FirstName} {message.Chat.LastName} #{message.Chat.Id}] execute GetMetricsCommand");

            Keyboards.Culture = Culture;

            var userId = await (await RequestClient.Instance.Client!.GetAsync($"https://localhost:8081/api/Cache/{message?.Chat.Id}_current_user_id")).Content.ReadAsStringAsync();
            var dto = new SSHRequestDto
            {
                VirtualMachine = await RequestClient.Instance.GetCachedAsync<VirtualMachine>($"{message?.Chat.Id}_vm"),
                UserId = userId,
                TelegramId = message!.Chat.Id
            };
            var metrics = await RequestClient.Instance.GetMetricsAsync(dto);

            if (message!.Text!.Contains("-r") || message!.Text!.Contains("--raw"))
            {
                await client.SendTextMessageAsync(message!.Chat.Id, $"{LocalizationManager.GetString("RawData", Culture)}:```\n{JsonConvert.SerializeObject(metrics)}```", parseMode: ParseMode.MarkdownV2, replyMarkup: Keyboards.VMActionKeyboard);

                return;
            }

            var images = new List<string>();
            var cpuPercentage = new GraphDto
            {
                Name = "cpuPercentage",
                Title = "CPU usage percentage",
                Labels = metrics!.CpuDto!.CpuPercentage!.GetType().GetProperties().Select(_ => _.Name).ToList(),
                Values = metrics!.CpuDto!.CpuPercentage!.GetType().GetProperties().Select(_ => float.Parse(_.GetValue(metrics!.CpuDto!.CpuPercentage)!.ToString() ?? "0.0")).ToList(),
                UserId = userId
            };
            var discBusy = new GraphDto
            {
                Name = "discBusy",
                Title = "Discs busy",
                Labels = metrics.DiscDto!.DiskBusy!.Select(_ => _.Key).ToList(),
                Values = metrics.DiscDto!.DiskBusy!.Select(_ => _.Value).ToList(),
                UserId = userId
            };
            var memory = new GraphDto
            {
                Name = "memory",
                Title = "Memory usage",
                Labels = metrics!.MemDto!.GetType().GetProperties().Select(_ => _.Name).ToList(),
                Values = metrics!.MemDto!.GetType().GetProperties().Select(_ => float.Parse(_.GetValue(metrics!.MemDto)!.ToString() ?? "0.0")).ToList(),
                UserId = userId
            };

            images.Add(GraphsService.CreateGraph(GraphType.Donut, cpuPercentage));
            images.Add(GraphsService.CreateGraph(GraphType.StackedBar, discBusy));
            images.Add(GraphsService.CreateGraph(GraphType.Pie, memory));
            
            foreach (var _ in metrics.DiscDto.DiskReadsWrites!)
            {
                var graphDto = new GraphDto
                {
                    Name = $"DiskReadsWrites_{_.Key}",
                    Title = $"{_.Key} reads/writes",
                    Labels = [ "reads", "writes" ],
                    Values = _.Value.Select(_ => float.Parse(_.ToString())).ToList(),
                    UserId = userId
                };

                images.Add(GraphsService.CreateGraph(GraphType.StackedBar, graphDto));
            }

            foreach (var _ in metrics.DiscDto.DiskReadsWritesPersec!)
            {
                var graphDto = new GraphDto
                {
                    Name = $"DiskReadsWritesPersec_{_.Key}",
                    Title = $"{_.Key} reads/writes per sec",
                    Labels = ["reads", "writes"],
                    Values = _.Value.Select(_ => float.Parse(_.ToString())).ToList(),
                    UserId = userId
                };

                images.Add(GraphsService.CreateGraph(GraphType.StackedBar, graphDto));
            }

            foreach (var _ in metrics.NetDto!.RxTxBits!)
            {
                var graphDto = new GraphDto
                {
                    Name = $"RxTxBits_{_.Key}",
                    Title = $"{_.Key} bits sent/received",
                    Labels = ["sent", "received"],
                    Values = _.Value.Select(_ => float.Parse(_.ToString())).ToList(),
                    UserId = userId
                };

                images.Add(GraphsService.CreateGraph(GraphType.StackedBar, graphDto));
            }

            foreach (var _ in images)
            {
                using (var fileStream = FileManager.OpenFileAsStream(_))
                {
                    await client.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(fileStream), parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
                    //await client.SendTextMessageAsync(message.Chat.Id, $"{string.Join("\n", metrics.CpuDto.CpuInfo!.Select(_ => $"<b>{_.Key}:</b> {_.Value}"))}", 
                    //    parseMode: ParseMode.Html, replyMarkup: Keyboards.VMActionKeyboard);
                }

                GraphsService.DeleteGraphFromLocal(_);
            }
        }

        public override Task<bool> TryExecuteAsync(ITelegramBotClient client, Message? message)
        {
            Names = [LocalizationManager.GetString("Metrics", Culture), "/metrics"];

            return base.TryExecuteAsync(client, message);
        }
    }
}
