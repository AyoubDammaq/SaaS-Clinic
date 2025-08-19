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
using RDV.Application.Queries.CountPendingRDVByClinic;
using RDV.Application.Queries.CountPendingRDVByDoctor;
using RDV.Application.Queries.GetAllRendezVous;
using RDV.Application.Queries.GetNombreRendezVousByClinicAndToday;
using RDV.Application.Queries.GetNombreRendezVousByMedecinAndToday;
using RDV.Application.Queries.GetNombreRendezVousParCliniqueEtDate;
using RDV.Application.Queries.GetRendezVousByDate;
using RDV.Application.Queries.GetRendezVousById;
using RDV.Application.Queries.GetRendezVousByMedecinId;
using RDV.Application.Queries.GetRendezVousByPatientId;
using RDV.Application.Queries.GetRendezVousByPeriodByClinic;
using RDV.Application.Queries.GetRendezVousByPeriodByDoctor;
using RDV.Application.Queries.GetRendezVousByPeriodByPatient;
using RDV.Application.Queries.GetRendezVousByStatut;
using RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByClinic;
using RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByDoctor;
using RDV.Application.Queries.GetRendezVousParMedecinEtDate;
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

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
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
        public async Task<IActionResult> CreateRendezVous([FromBody] CreateRendezVousDto rendezVous)
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
                return StatusCode(StatusCodes.Status201Created);
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

        [HttpGet("medecin/{medecinId}/date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByMedecinAndDate(Guid medecinId, [FromQuery] DateTime date)
        {
            var rdvs = await _mediator.Send(new GetRendezVousParMedecinEtDateQuery(medecinId, date));
            return Ok(rdvs);
        }

        [HttpGet("countByDoctorToday/{medecinId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNombreRendezVousByMedecinAndToday(Guid medecinId, [FromQuery] DateTime date)
        {
            try
            {
                if (medecinId == Guid.Empty || date == default)
                {
                    return BadRequest("Medecin ID et date sont requis.");
                }
                var count = await _mediator.Send(new GetNombreRendezVousByMedecinAndTodayQuery(medecinId, date));
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpGet("countByClinicToday/{clinicId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNombreRendezVousByClinicAndToday(Guid clinicId, [FromQuery] DateTime date)
        {
            try
            {
                if (clinicId == Guid.Empty || date == default)
                {
                    return BadRequest("Clinic ID et date sont requis.");
                }
                var count = await _mediator.Send(new GetNombreRendezVousByClinicAndTodayQuery(clinicId, date));
                return Ok(count);
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
        public async Task<IActionResult> AnnulerRendezVousParMedecin(Guid rendezVousId, [FromBody] JustificationDto dto)
        {
            try
            {
                if (rendezVousId == Guid.Empty)
                {
                    return BadRequest("Invalid rendez-vous ID");
                }

                await _mediator.Send(new AnnulerRendezVousParMedecinCommand(rendezVousId, dto.Justification));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("count/pending/doctor/{medecinId}")]
        public async Task<ActionResult<int>> CountPendingRDVByDoctor(Guid medecinId)
        {
            try
            {
                if (medecinId == Guid.Empty)
                {
                    return BadRequest("Invalid medecin ID");
                }
                var count = await _mediator.Send(new CountPendingRDVByDoctorQuery(medecinId));
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("count/pending/clinic/{clinicId}")]
        public async Task<ActionResult<int>> CountPendingRDVByClinic(Guid clinicId)
        {
            try
            {
                if (clinicId == Guid.Empty)
                {
                    return BadRequest("Invalid clinic ID");
                }
                var count = await _mediator.Send(new CountPendingRDVByClinicQuery(clinicId));
                return Ok(count);
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

        [HttpGet("period/patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByPeriodByPatient(Guid patientId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                if (patientId == Guid.Empty || start == default || end == default)
                {
                    return BadRequest("Patient ID, start date and end date are required.");
                }
                var rendezVous = await _mediator.Send(new GetRendezVousByPeriodByPatientQuery(patientId, start, end));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpGet("period/doctor/{medecinId}")]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByPeriodByDoctor(Guid medecinId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                if (medecinId == Guid.Empty || start == default || end == default)
                {
                    return BadRequest("Medecin ID, start date and end date are required.");
                }
                var rendezVous = await _mediator.Send(new GetRendezVousByPeriodByDoctorQuery(medecinId, start, end));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpGet("period/clinic/{cliniqueId}")]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVousByPeriodByClinic(Guid cliniqueId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                if (cliniqueId == Guid.Empty || start == default || end == default)
                {
                    return BadRequest("Clinic ID, start date and end date are required.");
                }
                var rendezVous = await _mediator.Send(new GetRendezVousByPeriodByClinicQuery(cliniqueId, start, end));
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }



        //[Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
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

        //[Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
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

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("statistiques/hebdomadaire/by-doctor/{medecinId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVousHebdoStatDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVousHebdoStatDto>>> GetStatistiquesHebdomadairesByDoctor(Guid medecinId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                if (start == default || end == default)
                {
                    return BadRequest("Les dates de début et de fin sont requises.");
                }

                var stats = await _mediator.Send(new GetRendezVousHebdomadaireStatistiquesByDoctorQuery(medecinId, start, end));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("count/by-clinic/{cliniqueId}/date")]
        public async Task<ActionResult<int>> GetNombreRendezVousParCliniqueEtDate(Guid cliniqueId, DateTime date)
        {
            try
            {
                if (cliniqueId == Guid.Empty || date == default)
                {
                    return BadRequest("Clinique ID et date sont requis.");
                }
                var count = await _mediator.Send(new GetNombreRendezVousParCliniqueEtDateQuery(cliniqueId, date));
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("statistiques/hebdomadaire/by-clinic/{cliniqueId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVousHebdoStatDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVousHebdoStatDto>>> GetStatistiquesHebdomadairesByClinic(Guid cliniqueId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                if (start == default || end == default)
                {
                    return BadRequest("Les dates de début et de fin sont requises.");
                }

                var stats = await _mediator.Send(new GetRendezVousHebdomadaireStatistiquesByClinicQuery(cliniqueId, start, end));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }
    }
}
