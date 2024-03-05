using Core.Dtos;

namespace UserInfrastructure.Service.Interfaces
{
    public interface IUserService
    {
        Task<bool> CheckIfUserHasAccountAsync(long telegramId);
        Task<bool> CheckIfAccountWithUserNameExistAsync(string userName);
        Task<AuthResponse> LoginAsync(LoginDto dto);
    }

    public enum AuthResponse
    {
        SuccessesLogin,
        SuccessesRegister,
        BadCredentials,
        AlreadyRegistered
    }
}
