using Core.Dtos;

namespace Infrastructure.Services.Interfaces
{
    public interface ISFTPRequestService
    {
        Task<string> CreateDirectoryAsync(SFTPRequestDto dto);
        Task<string> DeleteDirectoryAsync(SFTPRequestDto dto);
    }
}
