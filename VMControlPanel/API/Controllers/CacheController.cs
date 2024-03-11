using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _service;

        public CacheController(ICacheService service)
        {
            _service = service;
        }

        [HttpGet("{key}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetValueAsync(string key) 
        {
            return Ok(await _service.GetValueAsync<string>(key));
        }

        [HttpPost("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetValueAsync(string key, string value, float expTimeInHours)
        {
            await _service.SetValueAsync(key, value, expTimeInHours);

            return Ok();
        }

        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> RemoveDataAsync(string key)
        {
            await _service.RemoveDataAsync(key);

            return Ok();
        }
    }
}
