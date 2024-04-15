namespace Infrastructure.Services.Interfaces
{
    public interface ICacheService
    {
        Task SetValueAsync<T>(string key, T value, float expTimeInHours);
        Task<T?> GetValueAsync<T>(string key);
        Task RemoveDataAsync(string key);
    }
}
