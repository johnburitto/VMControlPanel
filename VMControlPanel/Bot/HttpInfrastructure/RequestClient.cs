using Bot.StateMachineBase;
using Core.Dtos;
using Core.Dtos.Metrics;
using Core.Entities;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using UserInfrastructure.Service.Interfaces;

namespace Bot.HttpInfrastructure
{
    public static class RequestClient
    {
        private static HttpClient? _client;
        private static readonly object _lock = new();

        public static HttpClient? Client => GetInstance();

        private static HttpClient? GetInstance()
        {
            if (_client == null)
            {
                lock (_lock)
                {
                    _client = new HttpClient();
                }
            }

            return _client;
        }

        public static async Task<List<User>?> GetUserAccountsAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8080/api/Auth/accounts/{telegramId}");
            
            return JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> GetStateAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetStateNameAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
            var obj = JsonConvert.DeserializeObject<State>(await response.Content.ReadAsStringAsync());

            return obj?.StateName ?? "";
        }

        public static async Task SaveStateAsync(long telegramId, State state, float expTimeInHours)
        {
            var stateSting = JsonConvert.SerializeObject(state);

            await Client!.PostAsync($"https://localhost:8081/api/Cache/{telegramId}_state?value={stateSting}&expTimeInHours={expTimeInHours}", null);
        }

        public static async Task RemoveStateAsync(long telegramId)
        {
            await Client!.DeleteAsync($"https://localhost:8081/api/Cache/{telegramId}_state");
        }

        public static async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8080/api/Auth/login", content);

            return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<AuthResponse> RegisterAsync(RegisterDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8080/api/Auth/register", content);

            return JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
        }

        public static async Task CacheAsync(string key, string value, float expTimeInHours)
        {
            await Client!.PostAsync($"https://localhost:8081/api/Cache/{key}?value={value}&expTimeInHours={expTimeInHours}", null);
        }

        public static async Task<T?> GetCachedAsync<T>(string key)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{key}");

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public static async Task DeleteCachedAsync(string key)
        {
            var response = await Client!.DeleteAsync($"https://localhost:8081/api/Cache/{key}");
        }

        public static async Task<bool> CheckIfHasCacheAsync(string key)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{key}");
            var data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            return data != null;
        }

        public static async Task<List<VirtualMachine>> GetUserVirtualMachinesAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var virtualMachinesResponse = await Client!.GetAsync($"https://localhost:8081/api/VirtualMachine/{userId}/all");

            return JsonConvert.DeserializeObject<List<VirtualMachine>>(await virtualMachinesResponse.Content.ReadAsStringAsync()) ?? [];
        }
        
        public static async Task<List<string>> GetUserVirtualMachinesNamesAsync(long telegramId)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var virtualMachinesResponse = await Client!.GetAsync($"https://localhost:8081/api/VirtualMachine/{userId}/all");

            return JsonConvert.DeserializeObject<List<VirtualMachine>>(await virtualMachinesResponse.Content.ReadAsStringAsync())?.Select(_ => _.Name ?? "").ToList() ?? [];
        }

        public static async Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(long telegramId, string? name)
        {
            var response = await Client!.GetAsync($"https://localhost:8081/api/Cache/{telegramId}_current_user_id");
            var userId = await response.Content.ReadAsStringAsync();
            var virtualMachinesResponse = await Client!.GetAsync($"https://localhost:8081/api/VirtualMachine/{userId}/{name}");

            return JsonConvert.DeserializeObject<VirtualMachine>(await virtualMachinesResponse.Content.ReadAsStringAsync());
        }

        public static async Task<VirtualMachine?> AddVirtualMachineAsync(VirtualMachineDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/VirtualMachine", content);

            return JsonConvert.DeserializeObject<VirtualMachine>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> ExecuteSSHCommandAsync(SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SSHRequest", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<MetricsDto?> GetMetricsAsync(SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SSHRequest/metrics", content);

            return JsonConvert.DeserializeObject<MetricsDto>(await response.Content.ReadAsStringAsync());
        }

        public static async Task DisposeClientAndStream(SSHRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SSHRequest/exit", content);
        }

        public static async Task<string> CreateDirectoryAsync(SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/directory/create", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<string> DeleteDirectoryAsync(SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/directory/delete", content);

            return Regex.Replace(await response.Content.ReadAsStringAsync(), @"\x1B\[[^@-~]*[@-~]", "");
        }

        public static async Task<FileDto?> GetFileFromVirtualMachineAsync(SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/file/get", content);

            return JsonConvert.DeserializeObject<FileDto>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<string> UploadFileToVirtualMachine(SFTPRequestDto dto)
        {
            var dtoString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(dtoString, Encoding.UTF8, "application/json");
            var response = await Client!.PostAsync($"https://localhost:8081/api/SFTPRequest/file/upload", content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
