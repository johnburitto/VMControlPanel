using Core.Entities;

namespace Core.Dtos
{
    public class RegisterDto
    {
        public long TelegramId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public Cultures Culture { get; set; }
    }
}
