using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Utilities;

namespace API.Profiles
{
    public class VirtualMachineProfile : Profile
    {
        public VirtualMachineProfile() 
        {
            CreateMap<VirtualMachineDto, VirtualMachine>()
                .ForMember(dest => dest.PasswordEncrypted, options => options.MapFrom(src => CryptoService.Blowfish(src.Password, true)))
                .ForMember(dest => dest.Password, options => options.Ignore());
        }
    }
}
