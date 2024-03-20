using Core.Dtos;
using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface IVirtualMachineService : ICrudService<VirtualMachine, VirtualMachineDto, VirtualMachineDto>
    {
        Task<VirtualMachine?> GetVirtualMachineByNameAndUserTelegramId(string? name, long userTelegramId);
    }
}
