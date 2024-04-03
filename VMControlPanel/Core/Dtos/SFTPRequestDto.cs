using Core.Entities;

namespace Core.Dtos
{
    public class SFTPRequestDto
    {
        public VirtualMachine? VirtualMachine { get; set; }
        public string? Data { get; set; }
        public object? File { get; set; }
        public string? UserId { get; set; }
    }
}
