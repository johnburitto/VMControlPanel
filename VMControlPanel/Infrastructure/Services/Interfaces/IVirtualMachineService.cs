using Core.Dtos;
using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface IVirtualMachineService : ICrudService<VirtualMachine, VirtualMachineDto, VirtualMachineDto>
    {
        Task<VirtualMachine?> GetVirtualMachineByUserTelegramIdAndNameAsync(long userTelegramId, string? name);
    }
}
