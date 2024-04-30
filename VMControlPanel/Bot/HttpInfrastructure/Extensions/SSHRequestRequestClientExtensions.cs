using Core.Dtos.Metrics;
using Core.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;

namespace Bot.HttpInfrastructure.Extensions
{
    public static class SSHRequestRequestClientExtensions
    {
        public static async Task<string> ExecuteSSHCommandAsync(this RequestClient client, SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.ApiUrl}/SSHRequest", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<MetricsDto?> GetMetricsAsync(this RequestClient client, SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.ApiUrl}/SSHRequest/metrics", content);

            return JsonConvert.DeserializeObject<MetricsDto>(await response.Content.ReadAsStringAsync());
        }

        public static async Task DisposeClientAndStream(this RequestClient client, SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var token = await client.GetCachedAsync($"{dto.TelegramId}_auth");

            client.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.Client!.PostAsync($"{client.ApiConfiguration!.ApiUrl}/SSHRequest/exit", content);
        }
    }
}
