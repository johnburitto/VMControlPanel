using Core.Dtos;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSHRequestController : ControllerBase
    {
        private readonly ISSHRequestService _service;

        public SSHRequestController(ISSHRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ExecuteCommandAsync(SSHRequestDto dto, CommandType type)
        {
            return Ok(await _service.ExecuteCommandAsync(dto.VirtualMachine!, dto.Command!, type));
        }
    }
}
