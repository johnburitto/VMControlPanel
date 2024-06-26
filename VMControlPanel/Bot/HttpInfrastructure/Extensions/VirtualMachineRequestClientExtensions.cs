﻿using Core.Dtos;
using Core.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class VirtualMachineRequestClientExtensions
    {
        public static async Task<List<VirtualMachine>> GetUserVirtualMachinesAsync(this RequestClient client, long telegramId)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var token = await client.GetCachedAsync($"{telegramId}_auth");

            client.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var virtualMachinesResponse = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine/{userId}/all");

            return JsonConvert.DeserializeObject<List<VirtualMachine>>(await virtualMachinesResponse.Content.ReadAsStringAsync()) ?? [];
        }

        public static async Task<List<string>> GetUserVirtualMachinesNamesAsync(this RequestClient client, long telegramId)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var token = await client.GetCachedAsync($"{telegramId}_auth");

            client.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var virtualMachinesResponse = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine/{userId}/all");

            return JsonConvert.DeserializeObject<List<VirtualMachine>>(await virtualMachinesResponse.Content.ReadAsStringAsync())?.Select(_ => _.Name ?? "").ToList() ?? [];
        }

        public static async Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(this RequestClient client, long telegramId, string? name)
        {
            var response = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var token = await client.GetCachedAsync($"{telegramId}_auth");

            client.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var virtualMachinesResponse = await client.Client!.GetAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine/{userId}/{name}");

            return JsonConvert.DeserializeObject<VirtualMachine>(await virtualMachinesResponse.Content.ReadAsStringAsync());
        }

        public static async Task<VirtualMachine?> AddVirtualMachineAsync(this RequestClient client, VirtualMachineDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine", content);

            return JsonConvert.DeserializeObject<VirtualMachine>(await response.Content.ReadAsStringAsync());
        }
        
        public static async Task<VirtualMachine?> UpdateVirtualMachineAsync(this RequestClient client, VirtualMachineDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PutAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine", content);

            return JsonConvert.DeserializeObject<VirtualMachine>(await response.Content.ReadAsStringAsync());
        }
        
        public static async Task<bool> DeleteVirtualMachineAsync(this RequestClient client, long telegramId)
        {
            var virtualMachine = await client.GetCachedAsync<VirtualMachine>($"{telegramId}_vm");
            var token = await client.GetCachedAsync($"{telegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.DeleteAsync($"{client.ApiConfiguration!.ApiUrl}/VirtualMachine/{virtualMachine?.Id}");

            return response.StatusCode == HttpStatusCode.NoContent ? true : false;
        }
    }
}
