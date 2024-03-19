using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Impls;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserInfrastructure.Data;
using UserInfrastructure.Service.Interfaces;

namespace UserInfrastructure.Service.Imls
{
    public class AuthService : IAuthService
    {
        private readonly UserDbContext _context;

        public AuthService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckIfUserHasAccountAsync(long telegramId)
        {
            return await _context.Users.Where(_ => _.TelegramId == telegramId).AnyAsync();
        }

        public async Task<bool> CheckIfAccountWithUserNameExistAsync(string? userName)
        {
            return await _context.Users.Where(_ => _.UserName == userName).AnyAsync();
        }

        public async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.Where(_ => _.UserName == dto.UserName && _.PasswordHash == CryptoService.ComputeSha256Hash(dto.Password)).FirstOrDefaultAsync();

            return user == null ? AuthResponse.BadCredentials : AuthResponse.SuccessesLogin;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
        {
            if (await CheckIfAccountWithUserNameExistAsync(dto.UserName))
            {
                return AuthResponse.AlreadyRegistered;
            }

            await CreateAsync(dto);

            return AuthResponse.SuccessesRegister;
        }

        public async Task<AuthResponse> RegisterAndLoginAsync(RegisterDto dto)
        {
            if (await RegisterAsync(dto) == AuthResponse.SuccessesRegister)
            {
                return await LoginAsync(new LoginDto
                {
                    UserName = dto.UserName,
                    Password = dto.Password
                });
            }

            return AuthResponse.AlreadyRegistered;
        }

        private async Task CreateAsync(RegisterDto dto)
        {
            var user = new User
            {
                TelegramId = dto.TelegramId,
                UserName = dto.UserName,
                PasswordHash = CryptoService.ComputeSha256Hash(dto.Password),
                Email = dto.Email
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersByTelegramIdAsync(long telegramId)
        {
            return await CheckIfUserHasAccountAsync(telegramId) ? await _context.Users.Where(_ => _.TelegramId == telegramId).ToListAsync() : [];
        }
    }
}
