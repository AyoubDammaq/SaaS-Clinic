using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RDVManagementService.Models;
using RDVManagementService.Services;

namespace RDVManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RendezVousController : ControllerBase
    {
        private readonly IRendezVousService _rendezVousService;

        public RendezVousController(IRendezVousService rendezVousService)
        {
            _rendezVousService = rendezVousService;
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RendezVous>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetAllRendezVous()
        {
            try
            {
                var rendezVous = await _rendezVousService.GetAllRendezVousAsync();
                if (rendezVous == null || !rendezVous.Any())
                {
                    return NotFound("Rendez-vous not found");
                }
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

                var rendezVous = await _rendezVousService.GetRendezVousByIdAsync(id);
                if (rendezVous == null)
                {
                    return NotFound("Rendez-vous non trouvé");
                }
                return Ok(rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRendezVous([FromBody] RendezVousDTO rendezVous)
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
                await _rendezVousService.CreateRendezVousAsync(rendezVous);
                return CreatedAtAction(nameof(GetRendezVousById), new { id = rendezVous.Id }, rendezVous);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRendezVous(Guid id, [FromBody] RendezVousDTO rendezVous)
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
                await _rendezVousService.UpdateRendezVousAsync(id, rendezVous);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

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

                var result = await _rendezVousService.AnnulerRendezVousAsync(id);
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

                var rendezVous = await _rendezVousService.GetRendezVousByPatientIdAsync(patientId);
                if (rendezVous == null || !rendezVous.Any())
                {
                    return NotFound("Rendez-vous non trouvé");
                }
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

                var rendezVous = await _rendezVousService.GetRendezVousByMedecinIdAsync(medecinId);
                if (rendezVous == null || !rendezVous.Any())
                {
                    return NotFound("Rendez-vous non trouvé");
                }
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
                var rendezVous = await _rendezVousService.GetRendezVousByDateAsync(date);
                if (rendezVous == null || !rendezVous.Any())
                {
                    return NotFound("Rendez-vous non trouvé");
                }
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
                var rendezVous = await _rendezVousService.GetRendezVousByStatutAsync(statut);
                if (rendezVous == null || !rendezVous.Any())
                {
                    return NotFound("Rendez-vous non trouvé");
                }
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

                await _rendezVousService.ConfirmerRendezVousParMedecin(rendezVousId);
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

                await _rendezVousService.AnnulerRendezVousParMedecin(rendezVousId, justification);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }
    }
}
