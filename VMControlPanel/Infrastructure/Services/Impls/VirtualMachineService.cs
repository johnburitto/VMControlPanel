using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Impls
{
    public class VirtualMachineService : IVirtualMachineService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public VirtualMachineService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VirtualMachine> CreateAsync(VirtualMachineDto dto)
        {
            var expectedEntity = await GetVirtualMachineByUserIdAndVMNameAsync(dto.UserId, dto.Name);
        
            if (expectedEntity != null)
            {
                throw new Exception($"Virtual machine with name {dto.Name} already added");
            }

            var entity = _mapper.Map<VirtualMachine>(dto);

            await _context.VirtualMachines.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(VirtualMachine entity)
        {
            _context.VirtualMachines.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<VirtualMachine>> GetAllAsync()
        {
            return _context.VirtualMachines.ToListAsync();
        }

        public Task<VirtualMachine?> GetByIdAsync(int id)
        {
            return _context.VirtualMachines.Where(_ => _.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<VirtualMachine>> GetUserVirtualMachinesAsync(string? userId)
        {
            return _context.VirtualMachines.Where(_ => _.UserId ==  userId).ToListAsync();
        }

        public Task<VirtualMachine?> GetVirtualMachineByUserIdAndVMNameAsync(string? userId, string? name)
        {
            return _context.VirtualMachines.Where(_ => _.UserId == userId && _.Name == name).FirstOrDefaultAsync();
        }

        public async Task<VirtualMachine> UpdateAsync(VirtualMachineDto dto)
        {
            var expectedEntity = await GetVirtualMachineByUserIdAndVMNameAsync(dto.UserId, dto.Name) ?? throw new Exception($"Virtual machine with name {dto.Name} doesn't exist");
            _mapper.Map(dto, expectedEntity);

            _context.VirtualMachines.Update(expectedEntity);
            await _context.SaveChangesAsync();

            return expectedEntity;
        }
    }
}
