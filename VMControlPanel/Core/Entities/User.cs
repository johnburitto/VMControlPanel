namespace Core.Entities
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long TelegramId { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? NormalizedUserName { get => UserName?.ToUpper(); set { } }
        public string? NormalizedEmail { get => Email?.ToUpper(); set { } }
    }
}
