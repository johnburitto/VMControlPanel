using Core.Entities;

namespace Core.Dtos
{
    public class SFTPRequestDto
    {
        public VirtualMachine? VirtualMachine { get; set; }
        public string? Data { get; set; }
        public string? FilePath { get; set; }
        public string? UserId { get; set; }
        public long TelegramId { get; set; }
    }
}
