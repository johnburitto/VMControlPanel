using Core.Dtos;
using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface IVirtualMachineService : ICrudService<VirtualMachine, VirtualMachineDto, VirtualMachineDto>
    {
        Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(long userTelegramId, string? name);
    }
}
