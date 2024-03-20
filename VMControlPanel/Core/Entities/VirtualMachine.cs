using Utilities;

namespace Core.Entities
{
    public class VirtualMachine
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long UserTelegramId { get; set; }
        public string? UserName { get; set; }
        public string? PasswordEncrypted { get; set; }
        public string? Password { get => CryptoService.Blowfish(PasswordEncrypted, false); set { } }
        public string? Host { get; set;}
        public int Port { get; set; }
    }
}
