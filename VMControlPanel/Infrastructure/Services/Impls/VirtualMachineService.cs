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
            var expectedEntity = await GetVirtualMachineByNameAndUserTelegramId(dto.Name, dto.UserTelegramId);
        
            if (expectedEntity != null)
            {
                throw new Exception($"Virtual machine with name {dto.Name} already added");
            }

            var entity = _mapper.Map<VirtualMachine>(dto);

            await _context.VirtualMachines.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                _context.VirtualMachines.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public Task<List<VirtualMachine>> GetAllAsync()
        {
            return _context.VirtualMachines.ToListAsync();
        }

        public Task<VirtualMachine?> GetByIdAsync(int id)
        {
            return _context.VirtualMachines.Where(_ => _.Id == id).FirstOrDefaultAsync();
        }

        public Task<VirtualMachine?> GetVirtualMachineByNameAndUserTelegramId(string? name, long userTelegramId)
        {
            return _context.VirtualMachines.Where(_ => _.Name == name && _.UserTelegramId == userTelegramId).FirstOrDefaultAsync();
        }

        public async Task<VirtualMachine> Update(VirtualMachineDto dto)
        {
            var expectedEntity = await GetVirtualMachineByNameAndUserTelegramId(dto.Name, dto.UserTelegramId) ?? throw new Exception($"Virtual machine with name {dto.Name} doesn't exist");
            _mapper.Map(dto, expectedEntity);

            _context.VirtualMachines.Update(expectedEntity);
            await _context.SaveChangesAsync();

            return expectedEntity;
        }
    }
}
