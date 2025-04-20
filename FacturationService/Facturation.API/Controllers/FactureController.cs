using Facturation.Application.DTOs;
using Facturation.Application.Services;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly IFactureService _factureService;

        public FactureController(IFactureService facturationService)
        {
            _factureService = facturationService ?? throw new ArgumentNullException(nameof(facturationService));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFactureById(Guid id)
        {
            try
            {
                var facture = await _factureService.GetFactureByIdAsync(id);
                if (facture == null)
                {
                    return NotFound($"Facture with ID {id} not found.");
                }
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
                var factures = await _factureService.GetAllFacturesAsync();
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
                await _factureService.AddFactureAsync(facture);
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
                await _factureService.UpdateFactureAsync(facture);
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
                var facture = await _factureService.GetFactureByIdAsync(id);
                if (facture == null)
                {
                    return NotFound($"Facture with ID {id} not found.");
                }

                await _factureService.DeleteFactureAsync(id);
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
                var factures = await _factureService.GetAllFacturesByRangeOfDateAsync(startDate, endDate);
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
                var factures = await _factureService.GetAllFacturesByStateAsync(status);
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
                var factures = await _factureService.GetAllFacturesByPatientIdAsync(patientId);
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
                var factures = await _factureService.GetAllFacturesByClinicIdAsync(clinicId);
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
            var facture = await _factureService.GetFactureByIdAsync(id);
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
                var pdfBytes = await _factureService.ExportToPdfAsync(pdfFacture);
                return File(pdfBytes, "application/pdf", $"Facture_{id}.pdf");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error generating the PDF.");
            }
        }
    }
}
