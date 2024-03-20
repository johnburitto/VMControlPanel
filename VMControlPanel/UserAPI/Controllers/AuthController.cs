using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UserInfrastructure.Service.Interfaces;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly ITokenGenerateService _tokenGenerateService;
        private readonly ICacheService _cacheService;

        public AuthController(IAuthService service, ITokenGenerateService tokenGenerateService, ICacheService cacheService)
        {
            _service = service;
            _tokenGenerateService = tokenGenerateService;
            _cacheService = cacheService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> LoginAsync(LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);

            if (result == AuthResponse.SuccessesLogin)
            {
                var token = _tokenGenerateService.GenerateToken(dto);

                await _cacheService.SetValueAsync($"{dto.TelegramId}_auth", token, 1f);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterDto dto)
        {
            return Ok(await _service.RegisterAsync(dto));
        }

        [HttpGet("accounts/{telegramId}")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<User>>> GetUsersByTelegramIdAsync(long telegramId)
        {
            return Ok(await _service.GetUsersByTelegramIdAsync(telegramId));
        }

        [HttpGet("{telegramId}/{userName}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User?>> GetUserByTelegramIdAndUserNameAsync(long telegramId, string userName)
        {
            return Ok(await _service.GetUserByTelegramIdAndUserNameAsync(telegramId, userName));
        }
    }
}
