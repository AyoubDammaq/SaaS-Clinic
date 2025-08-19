using Facturation.Application.DTOs;
using Facturation.Application.TarificationService.Commands.Add;
using Facturation.Application.TarificationService.Commands.Delete;
using Facturation.Application.TarificationService.Commands.Update;
using Facturation.Application.TarificationService.Queries.GetAll;
using Facturation.Application.TarificationService.Queries.GetByClinicId;
using Facturation.Application.TarificationService.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarificationController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly ILogger<TarificationController> _logger;
        public TarificationController(IMediator mediator, ILogger<TarificationController> logger)
        {
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetAllTarifications")]
        public async Task<IActionResult> GetAllTarifications()
        {
            try
            {
                var result = await _mediator.Send(new GetAllQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all tarifications.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddTarification")]
        public async Task<IActionResult> AddTarification([FromBody] AddTarificationRequest addTarificationRequest)
        {
            if (addTarificationRequest == null)
            {
                _logger.LogWarning("AddTarificationRequest is null.");
                return BadRequest("Invalid request data.");
            }
            try
            {
                await _mediator.Send(new AddCommand(addTarificationRequest));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a tarification.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateTarification")]
        public
            async Task<IActionResult> UpdateTarification([FromBody] UpdateTarificationRequest updateTarificationRequest)
        {
            if (updateTarificationRequest == null)
            {
                _logger.LogWarning("UpdateTarificationRequest is null.");
                return BadRequest("Invalid request data.");
            }
            try
            {
                await _mediator.Send(new UpdateCommand(updateTarificationRequest));
                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Tarification not found for update.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a tarification.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteTarification/{id}")]
        public async Task<IActionResult> DeleteTarification(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Tarification not found for deletion.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a tarification.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTarificationById/{id}")]
        public async Task<IActionResult> GetTarification(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetByIdQuery(id));
                if (result == null)
                    return NotFound($"Tarification with ID {id} not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tarification by ID: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTarificationByClinicId/{cliniqueId}")]
        public async Task<IActionResult> GetTarificationByClinicId(Guid cliniqueId)
        {
            try
            {
                var result = await _mediator.Send(new GetByClinicIdQuery(cliniqueId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tarifications by clinic ID: {ClinicId}", cliniqueId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
