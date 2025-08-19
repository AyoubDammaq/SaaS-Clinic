using Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite;
using Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilite;
using Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilitesParMedecinId;
using Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite;
using Doctor.Application.AvailibilityServices.Queries.GetCreneauxDisponibles;
using Doctor.Application.AvailibilityServices.Queries.GetDisponibilites;
using Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinId;
using Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinIdAndJour;
using Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles;
using Doctor.Application.AvailibilityServices.Queries.GetTotalAvailableTime;
using Doctor.Application.AvailibilityServices.Queries.IsAvailable;
using Doctor.Application.AvailibilityServices.Queries.ObtenirDisponibilitesDansIntervalle;
using Doctor.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibiliteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DisponibiliteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> AjouterDisponibilites([FromBody] Disponibilite disponibilite)
        {
            try
            { 
                await _mediator.Send(new AjouterDisponibiliteCommand(disponibilite));
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
                await _mediator.Send(new UpdateDisponibiliteCommand(disponibiliteId, disponibilite));
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
                await _mediator.Send(new SupprimerDisponibiliteCommand(disponibiliteId));
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
                var disponibilites = await _mediator.Send(new GetDisponibilitesQuery());
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
                var disponibilites = await _mediator.Send(new GetDisponibilitesByMedecinIdQuery(medecinId));
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
                var disponibilites = await _mediator.Send(new GetDisponibilitesByMedecinIdAndJourQuery(medecinId, jour)); 
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
                var medecins = await _mediator.Send(new GetMedecinsDisponiblesQuery(date, heureDebut, heureFin));
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }

        [HttpGet("medecins/{medecinId}/creneaux-disponibles")]
        public async Task<IActionResult> ObtenirCreneauxDisponibles(Guid medecinId, DateTime date)
        {
            try
            {
                var creneaux = await _mediator.Send(new GetCreneauxDisponiblesQuery(medecinId, date));
                return Ok(creneaux);
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
                var estDisponible = await _mediator.Send(new IsAvailableQuery(medecinId, dateTime));
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
                var tempsDisponible = await _mediator.Send(new GetTotalAvailableTimeQuery(medecinId, dateDebut, dateFin));
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
                var disponibilites = await _mediator.Send(new ObtenirDisponibilitesDansIntervalleQuery(medecinId, start, end));
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
                await _mediator.Send(new SupprimerDisponibilitesParMedecinIdCommand(medecinId));
                return Ok(new { Message = "Disponibilités supprimées avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite.", Details = ex.Message });
            }
        }
    }
}
