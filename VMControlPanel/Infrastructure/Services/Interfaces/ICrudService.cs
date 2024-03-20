namespace Infrastructure.Services.Interfaces
{
    public interface ICrudService<T, TCreate, TUpdate>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(TCreate dto);
        Task<T> Update(TUpdate dto);
        Task DeleteAsync(int id);
    }
}
