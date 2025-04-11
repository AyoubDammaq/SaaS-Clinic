using DoctorManagementService.Models;
using DoctorManagementService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibiliteController : ControllerBase
    {
        private readonly IDisponibiliteService _disponibiliteService;
        public DisponibiliteController(IDisponibiliteService disponibiliteService)
        {
            _disponibiliteService = disponibiliteService;
        }

        [HttpPost("{id}/disponibilites")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> AjouterDisponibilites(Guid id, [FromBody] Disponibilite disponibilite)
        {
            try
            {
                await _disponibiliteService.AjouterDisponibilite(id, disponibilite);
                return Ok("Disponibilités ajoutées avec succès !");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpDelete("disponibilites/{disponibiliteId}")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> SupprimerDisponibilite(Guid disponibiliteId)
        {
            try
            {
                await _disponibiliteService.SupprimerDisponibilite(disponibiliteId);
                return Ok(new { Message = "Disponibilité supprimée avec succès" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("disponibilites/{id}")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinId(Guid id)
        {
            try
            {
                var disponibilites = await _disponibiliteService.GetDisponibilitesByMedecinId(id);
                if (disponibilites == null || !disponibilites.Any())
                {
                    return NotFound(new { Message = "Aucune disponibilité trouvée pour ce médecin" });
                }
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("disponibilites")]
        public async Task<IActionResult> ObtenirTousLesDisponibilites()
        {
            try
            {
                var disponibilites = await _disponibiliteService.GetDisponibilites();
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("disponibilites/{id}/disponible")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinIdEtDate(Guid id, [FromQuery] DateTime date)
        {
            try
            {
                var disponibilites = await _disponibiliteService.ObtenirDisponibilitesParMedecinIdEtDate(id, date);
                if (disponibilites == null || !disponibilites.Any())
                {
                    return NotFound(new { Message = "Aucune disponibilité trouvée pour ce médecin à cette date" });
                }
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("disponibilites/{date}/medecin")]
        public async Task<IActionResult> ObtenirDisponibiliteAvecDate([FromRoute] DateTime date, [FromQuery] TimeSpan? heureDebut, [FromQuery] TimeSpan? heureFin)
        {
            try
            {
                if (heureDebut >= heureFin)
                {
                    return BadRequest(new { Message = "L'heure de début doit être inférieure à l'heure de fin." });
                }

                var disponibilite = await _disponibiliteService.ObtenirDisponibiliteAvecDate(date, heureDebut, heureFin);

                if (disponibilite == null)
                {
                    return NotFound(new { Message = "Aucune disponibilité trouvée pour ce médecin." });
                }

                return Ok(disponibilite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }
    }
}
