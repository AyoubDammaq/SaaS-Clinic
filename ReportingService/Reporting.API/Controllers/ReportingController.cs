using Microsoft.AspNetCore.Mvc;
using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;

namespace Reporting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportingController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet("consultations/count")]
        public async Task<IActionResult> GetNombreConsultations([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var result = await _reportingService.GetNombreConsultationsAsync(start, end);

                if (result == 0)
                {
                    return NotFound("Aucune statistique de consultations trouvée pour la période spécifiée.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("rendezvous/stats")]
        public async Task<IActionResult> GetStatistiquesRendezVous([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var result = await _reportingService.GetStatistiquesRendezVousAsync(start, end);

                if (result == null || !result.Any())
                {
                    return NotFound("Aucune statistique de rendez-vous trouvée pour la période spécifiée.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("patients/nouveaux")]
        public async Task<IActionResult> GetNombreNouveauxPatients([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetNombreNouveauxPatientsAsync(start, end);
            return Ok(result);
        }

        [HttpGet("medecins/specialite")]
        public async Task<IActionResult> GetNombreMedecinParSpecialite()
        {
            var result = await _reportingService.GetNombreMedecinParSpecialite();
            return Ok(result);
        }

        [HttpGet("medecins/clinique")]
        public async Task<IActionResult> GetNombreMedecinByClinique()
        {
            var result = await _reportingService.GetNombreMedecinByClinique();
            return Ok(result);
        }

        [HttpGet("medecins/specialite/clinique/{cliniqueId}")]
        public async Task<IActionResult> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId)
        {
            var result = await _reportingService.GetNombreMedecinBySpecialiteDansUneClinique(cliniqueId);
            return Ok(result);
        }


        [HttpGet("factures/status")]
        public async Task<IActionResult> GetNombreDeFactureByStatus()
        {
            var result = await _reportingService.GetNombreDeFactureByStatus();
            return Ok(result);
        }
        [HttpGet("factures/clinique")]
        public async Task<IActionResult> GetNombreDeFactureParClinique()
        {
            var result = await _reportingService.GetNombreDeFactureParClinique();
            return Ok(result);
        }
        [HttpGet("factures/status/clinique")]
        public async Task<IActionResult> GetNombreDeFactureParStatusParClinique()
        {
            var result = await _reportingService.GetNombreDeFactureParStatusParClinique();
            return Ok(result);
        }
        [HttpGet("factures/status/clinique/{cliniqueId}")]
        public async Task<IActionResult> GetNombreDeFacturesByStatusDansUneClinique(Guid cliniqueId)
        {
            var result = await _reportingService.GetNombreDeFacturesByStatusDansUneClinique(cliniqueId);
            return Ok(result);
        }

        [HttpGet("cliniques/count")]
        public async Task<IActionResult> GetNombreCliniques()
        {
            var result = await _reportingService.GetNombreDeCliniques();
            return Ok(result);
        }

        [HttpGet("cliniques/nouveaux")]
        public async Task<IActionResult> GetNombreNouveauxCliniques()
        {
            var result = await _reportingService.GetNombreNouvellesCliniquesDuMois();
            return Ok(result);
        }

        [HttpGet("cliniques/nouveaux/mois")]
        public async Task<IActionResult> GetNombreNouveauxCliniquesParMois()
        {
            var result = await _reportingService.GetNombreNouvellesCliniquesParMois();
            return Ok(result);
        }

        [HttpGet("cliniques/{cliniqueId}/stats")]
        public async Task<IActionResult> GetStatistiquesClinique(Guid cliniqueId)
        {
            var result = await _reportingService.GetStatistiquesClinique(cliniqueId);
            return Ok(result);
        }

        [HttpGet("medecins/{medecinId}/activites")]
        public async Task<IActionResult> GetActivitesMedecin(Guid medecinId)
        {
            var result = await _reportingService.GetActivitesMedecin(medecinId);
            return Ok(result);
        }






        [HttpGet("paiements/montant")]
        public async Task<IActionResult> GetMontantPaiements([FromQuery] string statut, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetMontantPaiementsAsync(statut, start, end);
            return Ok(result);
        }

        [HttpGet("factures/count")]
        public async Task<IActionResult> GetNombreFactures([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetNombreFacturesAsync(start, end);
            return Ok(result);
        }

        [HttpGet("factures/montant")]
        public async Task<IActionResult> GetMontantFactures([FromQuery] string statut, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetMontantFacturesAsync(statut, start, end);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetDashboardStatsAsync(start, end);
            return Ok(result);
        }
    }

}
