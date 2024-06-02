using Core.Dtos;
using Core.Entities;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VirtualMachineController : ControllerBase
    {
        private readonly IVirtualMachineService _service;

        public VirtualMachineController(IVirtualMachineService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VirtualMachine>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<VirtualMachine>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}", Name = "GetDiscountByIdAsync")]
        [ProducesResponseType(typeof(VirtualMachine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachine?>> GetByIdAsync(int id)
        {
            var entity = await _service.GetByIdAsync(id);

            return entity != null ? Ok(entity) : NotFound();
        }

        [HttpGet("{userId}/{name}")]
        [ProducesResponseType(typeof(VirtualMachine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachine?>> GetVirtualMachineByUserIdAndVMNameAsync(string userId, string name)
        {
            return Ok(await _service.GetVirtualMachineByUserIdAndVMNameAsync(userId, name));
        }

        [HttpGet("{userId}/all")]
        [ProducesResponseType(typeof(List<VirtualMachine>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<VirtualMachine>>> GetUserVirtualMachines(string userId)
        {
            return Ok(await _service.GetUserVirtualMachinesAsync(userId));
        }

        [HttpPost]
        [ProducesResponseType(typeof(VirtualMachine), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachine>> CreateAsync(VirtualMachineDto dto)
        {
            try
            {
                var entity = await _service.CreateAsync(dto);

                return CreatedAtRoute("GetDiscountByIdAsync", new { Id = entity.Id }, entity);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(VirtualMachine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachine>> UpdateAsync(VirtualMachineDto dto)
        {
            return Ok(await _service.UpdateAsync(dto));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var entity = await _service.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(entity);

            return NoContent();
        }
    }
}
