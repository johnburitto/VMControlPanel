using Core.Dtos;

namespace UserInfrastructure.Service.Interfaces
{
    public interface ITokenGenerateService
    {
        string GenerateToken(LoginDto dto);
    }
}
