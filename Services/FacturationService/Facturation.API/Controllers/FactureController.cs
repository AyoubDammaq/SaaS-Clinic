using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.AddFacture;
using Facturation.Application.FactureService.Commands.DeleteFacture;
using Facturation.Application.FactureService.Commands.ExportToPdf;
using Facturation.Application.FactureService.Commands.UpdateFacture;
using Facturation.Application.FactureService.Queries.GetAllFactures;
using Facturation.Application.FactureService.Queries.GetAllFacturesByRangeOfDate;
using Facturation.Application.FactureService.Queries.GetFactureByCliniqueId;
using Facturation.Application.FactureService.Queries.GetFactureByConsultationId;
using Facturation.Application.FactureService.Queries.GetFactureById;
using Facturation.Application.FactureService.Queries.GetFacturesByFilter;
using Facturation.Application.FactureService.Queries.GetFacturesByPatientId;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureByStatus;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureParClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique;
using Facturation.Application.FactureService.Queries.GetRevenusMensuels;
using Facturation.Application.FactureService.Queries.GetRevenusMensuelTrend;
using Facturation.Application.FactureService.Queries.GetStatistiquesFacturesParPeriode;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FactureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        public async Task<IActionResult> GetFactureById(Guid id)
        {
            try
            {
                var facture = await _mediator.Send(new GetFactureByIdQuery(id));
                return Ok(facture);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> GetAllFactures()
        {
            try
            {
                var factures = await _mediator.Send(new GetAllFacturesQuery());
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> AddFacture([FromBody] CreateFactureRequest facture)
        {
            if (facture == null)
            {
                return BadRequest("Facture object is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            try
            {
                await _mediator.Send(new AddFactureCommand(facture));
                return StatusCode(201, "Facture créée avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> UpdateFacture([FromBody] UpdateFactureRequest facture)
        {
            if (facture == null)
            {
                return BadRequest("Facture object is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            try
            {
                await _mediator.Send(new UpdateFactureCommand(facture));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> DeleteFacture(Guid id)
        {
            try
            {
                var facture = await _mediator.Send(new GetFactureByIdQuery(id));
                if (facture == null)
                {
                    return NotFound($"Facture with ID {id} not found.");
                }

                await _mediator.Send(new DeleteFactureCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("clinic/{cliniqueId}")]
        public async Task<IActionResult> GetFactureByCliniquId(Guid cliniqueId)
        {
            try
            {
                var facture = await _mediator.Send(new GetFactureByCliniqueIdQuery(cliniqueId));
                return Ok(facture);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetFactureByPatientId(Guid patientId)
        {
            try
            {
                var factures = await _mediator.Send(new GetFacturesByPatientIdQuery(patientId));
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("consultation/{consultationId}")]
        public async Task<IActionResult> GetFactureByConsultationId(Guid consultationId)
        {
            Console.WriteLine($"ConsultationId reçu: {consultationId}");
            // Ou avec ILogger si tu en utilises un
            try
            {
                var facture = await _mediator.Send(new GetFactureByConsultationIdQuery(consultationId));
                return Ok(facture);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("range")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        public async Task<IActionResult> GetAllFacturesByRangeOfDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be greater than end date.");
            }

            try
            {
                var factures = await _mediator.Send(new GetAllFacturesByRangeOfDateQuery(startDate, endDate));
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        [HttpGet("filtrer")]
        public async Task<IActionResult> FiltrerFactures([FromQuery] Guid? clinicId, [FromQuery] Guid? patientId, [FromQuery] FactureStatus? status)
        {
            var result = await _mediator.Send(new GetFacturesByFilterQuery(clinicId, patientId, status));
            return Ok(result);
        }

        [HttpGet("export/{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        public async Task<IActionResult> ExportFacture(Guid id)
        {
            var facture = await _mediator.Send(new GetFactureByIdQuery(id));
            if (facture == null)
                return NotFound("Facture introuvable.");

            var pdfFacture = new Facture
            {
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                Status = facture.Status
            };

            try
            {
                var pdfBytes = await _mediator.Send(new ExportToPdfCommand(pdfFacture));
                return File(pdfBytes, "application/pdf", $"Facture_{id}.pdf");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Error generating the PDF." + e);
            }
        }

        [HttpGet("stats/status")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetNombreDeFactureByStatus()
        {
            try
            {
                var stats = await _mediator.Send(new GetNombreDeFactureByStatusQuery());
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stats/clinic")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetNombreDeFactureParClinique()
        {
            try
            {
                var stats = await _mediator.Send(new GetNombreDeFactureParCliniqueQuery());
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stats/status/clinic")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetNombreDeFacturesByStatusParClinique()
        {
            try
            {
                var stats = await _mediator.Send(new GetNombreDeFacturesByStatusParCliniqueQuery());
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stats/status/clinic/{clinicId}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetNombreDeFacturesByStatusDansUneClinique(Guid clinicId)
        {
            try
            {
                var stats = await _mediator.Send(new GetNombreDeFacturesByStatusDansUneCliniqueQuery(clinicId));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        [HttpGet("statistiques")]
        public async Task<IActionResult> GetStatistiques([FromQuery] DateTime debut, [FromQuery] DateTime fin)
        {
            var result = await _mediator.Send(new GetStatistiquesFacturesParPeriodeQuery(debut, fin));
            return Ok(result);
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        [HttpGet("revenus-mensuels/{clinicId}")]
        public async Task<IActionResult> GetRevenusMensuels(Guid clinicId)
        {
            try
            {
                var revenus = await _mediator.Send(new GetRevenusMensuelsQuery(clinicId));
                return Ok(revenus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
        [HttpGet("revenu-mensuel-trend/{clinicId}")]
        public async Task<IActionResult> GetRevenusMensuelTrend(Guid clinicId)
        {
            try
            {
                var trend = await _mediator.Send(new GetRevenusMensuelTrendQuery(clinicId));
                return Ok(trend);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
