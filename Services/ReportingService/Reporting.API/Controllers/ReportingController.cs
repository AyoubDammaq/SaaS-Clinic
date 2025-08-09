using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Interfaces;
using Reporting.Application.Utils;

namespace Reporting.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly ILogger<ReportingController> _logger;

        public ReportingController(IReportingService reportingService, ILogger<ReportingController> logger)
        {
            _reportingService = reportingService;
            _logger = logger;
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
            catch (Exception)
            {
                return StatusCode(500, "Erreur interne du serveur.");
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
            catch (Exception)
            {
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        [HttpGet("patients/nouveaux/count")]
        public async Task<IActionResult> GetNombreNouveauxPatients([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var result = await _reportingService.GetNombreNouveauxPatientsAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de nouveaux patients.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        [HttpGet("medecins/specialites/count")]
        public async Task<IActionResult> GetNombreMedecinParSpecialite()
        {
            try
            {
                var result = await _reportingService.GetNombreMedecinParSpecialite();
                if (result == null || !result.Any())
                {
                    return NotFound("Aucune statistique de médecins par spécialité trouvée.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des médecins par spécialité.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        [HttpGet("medecins/cliniques/count")]
        public async Task<IActionResult> GetNombreMedecinByClinique()
        {
            var result = await _reportingService.GetNombreMedecinByClinique();
            return Ok(result);
        }

        [HttpGet("medecins/specialites/cliniques/{cliniqueId}/count")]
        public async Task<IActionResult> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId)
        {
            var result = await _reportingService.GetNombreMedecinBySpecialiteDansUneClinique(cliniqueId);
            return Ok(result);
        }

        [HttpGet("factures/status/count")]
        public async Task<IActionResult> GetNombreDeFactureByStatus()
        {
            var result = await _reportingService.GetNombreDeFactureByStatus();
            return Ok(result);
        }

        [HttpGet("factures/cliniques/count")]
        public async Task<IActionResult> GetNombreDeFactureParClinique()
        {
            var result = await _reportingService.GetNombreDeFactureParClinique();
            return Ok(result);
        }

        [HttpGet("factures/status/cliniques/count")]
        public async Task<IActionResult> GetNombreDeFactureParStatusParClinique()
        {
            var result = await _reportingService.GetNombreDeFactureParStatusParClinique();
            return Ok(result);
        }

        [HttpGet("factures/status/cliniques/{cliniqueId}/count")]
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

        [HttpGet("cliniques/nouveaux/count")]
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

        [HttpGet("factures/stats")]
        public async Task<IActionResult> GetStatistiquesFactures([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _reportingService.GetStatistiquesFacturesAsync(start, end);
            return Ok(result);
        }

        [HttpPost("cliniques/comparaison")]
        public async Task<IActionResult> ComparerCliniques([FromBody] List<Guid> cliniqueIds)
        {
            if (cliniqueIds == null || !cliniqueIds.Any())
                return BadRequest("Liste des cliniques vide.");

            var result = await _reportingService.ComparerCliniquesAsync(cliniqueIds);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] Guid? patientId = null,
            [FromQuery] Guid? medecinId = null,
            [FromQuery] Guid? cliniqueId = null)
        {
            try
            {
                var stats = await _reportingService.GetDashboardStatsAsync(start, end, patientId, medecinId, cliniqueId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'appel à GetDashboardStats");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("dashboard/pdf")]
        public async Task<IActionResult> ExportDashboardPdf([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var stats = await _reportingService.GetDashboardStatsAsync(start, end);
            if (stats == null)
                return BadRequest("Impossible de générer le PDF : statistiques introuvables.");
            var pdfBytes = DashboardPdfGenerator.Generate(stats);
            return File(pdfBytes, "application/pdf", $"dashboard_{start:yyyyMMdd}_{end:yyyyMMdd}.pdf");
        }

        [HttpGet("rendezvous/weekly-stats/by-doctor/{medecinId}")]
        public async Task<IActionResult> GetStatistiquesHebdomadairesRendezVousByDoctor(Guid medecinId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var result = await _reportingService.GetStatistiquesHebdomadairesRendezVousByDoctorAsync(medecinId, start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques hebdomadaires des rendez-vous.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        [HttpGet("rendezvous/weekly-stats/by-clinic/{cliniqueId}")]
        public async Task<IActionResult> GetStatistiquesHebdomadairesRendezVousByClinic(Guid cliniqueId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var result = await _reportingService.GetStatistiquesHebdomadairesRendezVousByClinicAsync(cliniqueId, start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des statistiques hebdomadaires des rendez-vous par clinique.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }   


        [HttpGet("dashboard/excel")]
        public async Task<IActionResult> ExportDashboardExcel([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var stats = await _reportingService.GetDashboardStatsAsync(start, end);
            var excelBytes = DashboardExcelGenerator.Generate(stats);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"dashboard_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx");
        }
    }

}
