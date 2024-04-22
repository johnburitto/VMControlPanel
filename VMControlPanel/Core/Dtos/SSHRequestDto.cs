using Core.Entities;

namespace Core.Dtos
{
    public class SSHRequestDto
    {
        public VirtualMachine? VirtualMachine { get; set; }
        public string? Command { get; set; }
        public string? UserId { get; set; }
        public long TelegramId { get; set; }
    }
}
