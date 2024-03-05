namespace UserInfrastructure.Service.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckIfUserHasAccountAsync(long telegramId);
    }
}
