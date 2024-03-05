using Microsoft.EntityFrameworkCore;
using UserInfrastructure.Data;
using UserInfrastructure.Service.Interfaces;

namespace UserInfrastructure.Service.Imls
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckIfUserHasAccountAsync(long telegramId)
        {
            return await _context.Users.Where(_ => _.TelegramId == telegramId).AnyAsync();
        }
    }
}
