using Microsoft.AspNetCore.Mvc;
using PatientManagementService.DTOs;
using PatientManagementService.Services;

namespace PatientManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DossierMedicalController : ControllerBase
    {
        public readonly IDossierMedicalService _dossierMedicalService;

        public DossierMedicalController(IDossierMedicalService dossierMedicalService)
        {
            _dossierMedicalService = dossierMedicalService;
        }

        [HttpGet("dossier-medical/{patientId}")]
        public async Task<IActionResult> GetDossierMedicalByPatientId(Guid patientId)
        {
            try
            {
                var dossierMedical = await _dossierMedicalService.GetDossierMedicalByPatientIdAsync(patientId);
                if (dossierMedical == null)
                {
                    return NotFound("Dossier médical not found.");
                }
                return Ok(dossierMedical);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("dossier-medical")]
        public async Task<IActionResult> AddDossierMedical([FromBody] DossierMedicalDTO dossierMedical)
        {
            try
            {
                if (dossierMedical == null)
                {
                    return BadRequest("Invalid dossier médical data.");
                }
                await _dossierMedicalService.AddDossierMedicalAsync(dossierMedical);
                return CreatedAtAction(nameof(GetDossierMedicalByPatientId), new { patientId = dossierMedical.PatientId }, dossierMedical);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("dossier-medical")]
        public async Task<IActionResult> UpdateDossierMedical([FromBody] DossierMedicalDTO dossierMedical)
        {
            try
            {
                if (dossierMedical == null)
                {
                    return BadRequest("Invalid dossier médical data.");
                }
                await _dossierMedicalService.UpdateDossierMedicalAsync(dossierMedical);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("dossier-medical/{dossierMedicalId}")]
        public async Task<IActionResult> DeleteDossierMedical(Guid dossierMedicalId)
        {
            try
            {
                var dossierMedical = await _dossierMedicalService.GetDossierMedicalByIdAsync(dossierMedicalId);
                if (dossierMedical == null)
                {
                    return NotFound("Dossier médical not found.");
                }
                await _dossierMedicalService.DeleteDossierMedicalAsync(dossierMedicalId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("dossiers-medicals")]
        public async Task<IActionResult> GetAllDossiersMedicals()
        {
            try
            {
                var dossiersMedicals = await _dossierMedicalService.GetAllDossiersMedicalsAsync();
                return Ok(dossiersMedicals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
