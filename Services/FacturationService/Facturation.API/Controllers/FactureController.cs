using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.AddFacture;
using Facturation.Application.FactureService.Commands.DeleteFacture;
using Facturation.Application.FactureService.Commands.ExportToPdf;
using Facturation.Application.FactureService.Commands.UpdateFacture;
using Facturation.Application.FactureService.Queries.GetAllFactures;
using Facturation.Application.FactureService.Queries.GetAllFacturesByClinicId;
using Facturation.Application.FactureService.Queries.GetAllFacturesByPatientId;
using Facturation.Application.FactureService.Queries.GetAllFacturesByRangeOfDate;
using Facturation.Application.FactureService.Queries.GetAllFacturesByState;
using Facturation.Application.FactureService.Queries.GetFactureById;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureByStatus;
using Facturation.Application.FactureService.Queries.GetNombreDeFactureParClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique;
using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using MediatR;
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

        [HttpGet("range")]
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

        [HttpGet("state")]
        public async Task<IActionResult> GetAllFacturesByState(FactureStatus status)
        {
            try
            {
                var factures = await _mediator.Send(new GetAllFacturesByStateQuery(status));
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAllFacturesByPatientId(Guid patientId)
        {
            try
            {
                var factures = await _mediator.Send(new GetAllFacturesByPatientIdQuery(patientId));
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<IActionResult> GetAllFacturesByClinicId(Guid clinicId)
        {
            try
            {
                var factures = await _mediator.Send(new GetAllFacturesByClinicIdQuery(clinicId));
                return Ok(factures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("export/{id}")]
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
            catch (Exception)
            {
                return StatusCode(500, "Error generating the PDF.");
            }
        }

        [HttpGet("stats/status")]
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
    }
}
