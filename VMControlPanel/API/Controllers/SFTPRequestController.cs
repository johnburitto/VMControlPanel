using Core.Dtos;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SFTPRequestController : ControllerBase
    {
        private readonly ISFTPRequestService _service;

        public SFTPRequestController(ISFTPRequestService service)
        {
            _service = service;
        }

        [HttpPost("directory/create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateDirectoryAsync(SFTPRequestDto dto)
        {
            return Ok(await _service.CreateDirectoryAsync(dto));
        }
        
        [HttpPost("directory/delete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> DeleteDirectoryAsync(SFTPRequestDto dto)
        {
            return Ok(await _service.DeleteDirectoryAsync(dto));
        }
        
        [HttpPost("file/get")]
        [ProducesResponseType(typeof(FileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FileDto>> GetFileAsync(SFTPRequestDto dto)
        {
            return Ok(await _service.GetFileAsync(dto));
        }
        
        [HttpPost("file/upload")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UploadFileAsync(SFTPRequestDto dto)
        {
            return Ok(await _service.UploadFileAsync(dto));
        }
    }
}
