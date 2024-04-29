using Core.Dtos;
using Core.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class SFTPRequestRequestClientExtensions
    {
        public static async Task<string> CreateDirectoryAsync(this RequestClient client, SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/directory/create", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<string> DeleteDirectoryAsync(this RequestClient client, SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/directory/delete", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<FileDto?> GetFileFromVirtualMachineAsync(this RequestClient client, SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/file/get", content);

            return JsonConvert.DeserializeObject<FileDto>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> UploadFileToVirtualMachine(this RequestClient client, SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/file/upload", content);

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task ChangeUserCultureAsync(this RequestClient client, string userId, Cultures culture)
        {
            await client.Client!.PutAsync($"https://localhost:8080/api/Auth/language/{userId}/{culture}", null);
        }
    }
}
