using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.API.Controllers
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

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> AjouterDisponibilites([FromBody] Disponibilite disponibilite)
        {
            try
            {
                await _disponibiliteService.AjouterDisponibilite(disponibilite);
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

        [HttpPut("{disponibiliteId}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> ModifierDisponibilite(Guid disponibiliteId, [FromBody] Disponibilite disponibilite)
        {
            try
            {
                if (disponibiliteId != disponibilite.Id)
                {
                    return BadRequest(new { Message = "L'identifiant de la disponibilité ne correspond pas." });
                }
                await _disponibiliteService.UpdateDisponibilite(disponibilite);
                return Ok(new { Message = "Disponibilité mise à jour avec succès" });
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


        [HttpDelete("{disponibiliteId}")]
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

        [HttpGet]
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

        [HttpGet("medecins/{medecinId}")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinId(Guid medecinId)
        {
            try
            {
                var disponibilites = await _disponibiliteService.GetDisponibilitesByMedecinId(medecinId);
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("medecins/{medecinId}/jour/{jour}")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinIdEtJour(Guid medecinId, [FromRoute] DayOfWeek jour)
        {
            try
            {
                var disponibilites = await _disponibiliteService.GetDisponibilitesByMedecinIdAndJour(medecinId, jour);
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("medecins-disponibles")]
        public async Task<IActionResult> ObtenirMedecinsDisponible([FromQuery] DateTime date, [FromQuery] TimeSpan? heureDebut, [FromQuery] TimeSpan? heureFin)
        {
            try
            {
                if (heureDebut >= heureFin)
                {
                    return BadRequest(new { Message = "L'heure de début doit être inférieure à l'heure de fin." });
                }
                var medecins = await _disponibiliteService.GetMedecinsDisponibles(date, heureDebut, heureFin);
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("disponibilites/{medecinId}/disponible")]
        public async Task<IActionResult> EstDisponible(Guid medecinId, DateTime dateTime)
        {
            try
            {
                var estDisponible = await _disponibiliteService.IsAvailable(medecinId, dateTime);
                return Ok(new { Disponible = estDisponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("medecins/{medecinId}/total-temps-disponible")]
        public async Task<IActionResult> ObtenirTempsDisponible(Guid medecinId, DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                var tempsDisponible = await _disponibiliteService.GetTotalAvailableTime(medecinId, dateDebut, dateFin);
                return Ok(new { TotalAvailableTime = tempsDisponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("medecins/{medecinId}/intervalle")]
        public async Task<ActionResult<List<Disponibilite>>> ObtenirIntervalleDisponibilite(Guid medecinId, DateTime start, DateTime end)
        {
            try
            {
                var disponibilites = await _disponibiliteService.ObtenirDisponibilitesDansIntervalle(medecinId, start, end);
                return Ok(disponibilites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpDelete("medecins/{medecinId}")]
        public async Task<IActionResult> SupprimerDisponibilitesParMedecinId(Guid medecinId)
        {
            try
            {
                await _disponibiliteService.SupprimerDisponibilitesParMedecinId(medecinId);
                return Ok(new { Message = "Disponibilités supprimées avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }
    }
}
