using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RDV.Application.Commands.AnnulerRendezVous;
using RDV.Application.Commands.AnnulerRendezVousParMedecin;
using RDV.Application.Commands.ConfirmerRendezVousParMedecin;
using RDV.Application.Commands.CreateRendezVous;
using RDV.Application.Commands.UpdateRendezVous;
using RDV.Application.DTOs;
using RDV.Application.Queries.CountByMedecinIds;
using RDV.Application.Queries.CountDistinctPatientsByMedecinIds;
using RDV.Application.Queries.GetAllRendezVous;
using RDV.Application.Queries.GetRendezVousByDate;
using RDV.Application.Queries.GetRendezVousById;
using RDV.Application.Queries.GetRendezVousByMedecinId;
using RDV.Application.Queries.GetRendezVousByPatientId;
using RDV.Application.Queries.GetRendezVousByStatut;
using RDV.Application.Queries.GetStatistiques;
using RDV.Domain.Entities;
using RDV.Domain.Enums;

namespace RDVManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RendezVousController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RendezVousController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVousDTO>>> GetAllRendezVous()
        {
            try
            {
                var rendezVous = await _mediator.Send(new GetAllRendezVousQuery());
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RendezVous))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RendezVous>> GetRendezVousById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }

                var rendezVous = await _mediator.Send(new GetRendezVousByIdQuery(id));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRendezVous([FromBody] RendezVous rendezVous)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (rendezVous == null)
                {
                    return BadRequest("Rendez-vous ne peut pas être nul");
                }
                await _mediator.Send(new CreateRendezVousCommand(rendezVous));
                return CreatedAtAction(nameof(GetRendezVousById), new { id = rendezVous.Id }, rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRendezVous(Guid id, [FromBody] RendezVous rendezVous)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (rendezVous == null)
                {
                    return BadRequest("Rendez-vous ne peut pas être nul");
                }
                await _mediator.Send(new UpdateRendezVousCommand(id, rendezVous));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpPost("annuler/patient/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AnnulerRendezVous(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }

                var result = await _mediator.Send(new AnnulerRendezVousCommand(id));
                if (!result)
                {
                    return NotFound("Rendez-vous non trouvé ou déjà annulé");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByPatientId(Guid patientId)
        {
            try
            {
                if (patientId == Guid.Empty)
                {
                    return BadRequest("Invalid patient ID");
                }

                var rendezVous = await _mediator.Send(new GetRendezVousByPatientIdQuery(patientId));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("medecin/{medecinId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByMedecinId(Guid medecinId)
        {
            try
            {
                if (medecinId == Guid.Empty)
                {
                    return BadRequest("Invalid medecin ID");
                }

                var rendezVous = await _mediator.Send(new GetRendezVousByMedecinIdQuery(medecinId));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByDate(DateTime date)
        {
            try
            {
                var rendezVous = await _mediator.Send(new GetRendezVousByDateQuery(date));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("statut/{statut}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByStatut(RDVstatus statut)
        {
            try
            {
                var rendezVous = await _mediator.Send(new GetRendezVousByStatutQuery(statut));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpPost("confirmer/{rendezVousId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmerRendezVousParMedecin(Guid rendezVousId)
        {
            try
            {
                if (rendezVousId == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }

                await _mediator.Send(new ConfirmerRendezVousParMedecinCommand(rendezVousId));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpPost("annuler/medecin/{rendezVousId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AnnulerRendezVousParMedecin(Guid rendezVousId, [FromBody] string justification)
        {
            try
            {
                if (rendezVousId == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }

                await _mediator.Send(new AnnulerRendezVousParMedecinCommand(rendezVousId, justification));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetStats([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var rendezVous = await _mediator.Send(new GetStatistiquesQuery(start,end));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("count")]
        public async Task<ActionResult<int>> CountRendezVous([FromQuery] List<Guid> medecinIds)
        {
            try
            {
                var count = await _mediator.Send(new CountByMedecinIdsQuery(medecinIds));
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("distinct/patients")]
        public async Task<ActionResult<int>> CountDistinctPatients([FromQuery] List<Guid> medecinIds)
        {
            try
            {
                var count = await _mediator.Send(new CountDistinctPatientsByMedecinIdsQuery(medecinIds));
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

    }
}
