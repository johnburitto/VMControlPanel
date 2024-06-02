using Core.Dtos;
using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface IVirtualMachineService : ICrudService<VirtualMachine, VirtualMachineDto, VirtualMachineDto>
    {
        Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(string userId, string? name);
        Task<List<VirtualMachine>> GetUserVirtualMachinesAsync(string userId);
    }
}
