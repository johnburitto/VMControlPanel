using Core.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserInfrastructure.Configurations;
using UserInfrastructure.Service.Interfaces;

namespace UserInfrastructure.Service.Imls
{
    public class TokenGenerateService : ITokenGenerateService
    {
        private readonly TokenGenerateServiceConfiguration _configuration;

        public TokenGenerateService(IOptions<TokenGenerateServiceConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public string GenerateToken(LoginDto dto)
        {
            List<Claim> claims = [
                new Claim(ClaimTypes.Name, dto.UserName!),
                new Claim("telegram_id", $"{dto.TelegramId}")
            ];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenInfo = new JwtSecurityToken(issuer: _configuration.Issuer, audience: _configuration.Audience, claims: claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: cred);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

            return token;
        }
    }
}
