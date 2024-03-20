using Core.Dtos;
using Core.Entities;

namespace UserInfrastructure.Service.Interfaces
{
    public interface IAuthService
    {
        Task<bool> CheckIfUserHasAccountAsync(long telegramId);
        Task<bool> CheckIfAccountWithUserNameExistAsync(string? UserName);
        Task<AuthResponse> LoginAsync(LoginDto dto);
        Task<AuthResponse> RegisterAsync(RegisterDto dto);
        Task<AuthResponse> RegisterAndLoginAsync(RegisterDto dto);
        Task<List<User>> GetUsersByTelegramIdAsync(long telegramId);
    }

    public enum AuthResponse
    {
        SuccessesLogin,
        SuccessesRegister,
        BadCredentials,
        AlreadyRegistered
    }
}
