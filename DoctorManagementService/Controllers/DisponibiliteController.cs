using DoctorManagementService.Models;
using DoctorManagementService.Services;
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
        public async Task<IActionResult> AjouterDisponibilites(Guid id, [FromBody] Disponibilite disponibilite)
        {
            try
            {
                await _disponibiliteService.AjouterDisponibilite(id, disponibilite);
                return Ok("Disponibilités ajoutées avec succès !");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("disponibilites/{disponibiliteId}")]
        public async Task<IActionResult> SupprimerDisponibilite(Guid disponibiliteId)
        {
            await _disponibiliteService.SupprimerDisponibilite(disponibiliteId);
            return Ok(new { Message = "Disponibilité supprimée avec succès" });
        }

        [HttpGet("disponibilites/{id}")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinId(Guid id)
        {
            var disponibilites = await _disponibiliteService.GetDisponibilitesByMedecinId(id);
            if (disponibilites == null)
            {
                return NotFound(new { Message = "Aucune disponibilité trouvée pour ce médecin" });
            }
            return Ok(disponibilites);
        }
        [HttpGet("disponibilites")]
        public async Task<IActionResult> ObtenirTousLesDisponibilites()
        {
            var disponibilites = await _disponibiliteService.GetDisponibilites();
            return Ok(disponibilites);
        }
        [HttpGet("disponibilites/{id}/disponible")]
        public async Task<IActionResult> ObtenirDisponibilitesParMedecinIdEtDate(Guid id, [FromQuery] DateTime date)
        {
            var disponibilites = await _disponibiliteService.ObtenirDisponibilitesParMedecinIdEtDate(id, date);
            if (disponibilites == null)
            {
                return NotFound(new { Message = "Aucune disponibilité trouvée pour ce médecin à cette date" });
            }
            return Ok(disponibilites);
        }
    }
}
