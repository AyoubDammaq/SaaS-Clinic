using ConsultationManagementService.Data;
using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationController : ControllerBase
    {
        private readonly ConsultationDbContext _context;
        private readonly IConsultationService _consultationService;

        public ConsultationController(ConsultationDbContext context, IConsultationService consultationService)
        {
            _context = context;
            _consultationService = consultationService;
        }

        // GET: api/Consultation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Consultation>> GetConsultationByIdAsync(Guid id)
        {
            try
            {
                var consultation = await _consultationService.GetConsultationByIdAsync(id);
                if (consultation == null)
                {
                    return NotFound("Consultation not found");
                }

                return Ok(consultation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consultation>>> GetAllConsultationsAsync()
        {
            try
            {
                var consultations = await _consultationService.GetAllConsultationsAsync();
                return Ok(consultations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Consultation
        [HttpPost]
        public async Task<IActionResult> CreateConsultationAsync([FromBody] ConsultationDTO consultationDto)
        {
            try
            {
                if (consultationDto == null)
                {
                    return BadRequest("Invalid consultation data");
                }

                await _consultationService.CreateConsultationAsync(consultationDto);
                return Ok("Consultation créée avec succés");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Consultation
        [HttpPut]
        public async Task<IActionResult> UpdateConsultationAsync([FromBody] ConsultationDTO consultationDto)
        {
            try
            {
                if (consultationDto == null)
                {
                    return BadRequest("Invalid consultation data");
                }

                await _consultationService.UpdateConsultationAsync(consultationDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Consultation/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteConsultationAsync(Guid id)
        {
            try
            {
                var result = await _consultationService.DeleteConsultationAsync(id);
                if (!result)
                {
                    return NotFound("Consultation not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation/Document/{id}
        [HttpGet("Document/{id}")]
        public async Task<ActionResult<DocumentMedical>> GetDocumentMedicalByIdAsync(Guid id)
        {
            try
            {
                var documentMedical = await _consultationService.GetDocumentMedicalByIdAsync(id);
                if (documentMedical == null)
                {
                    return NotFound("Document médical not found");
                }

                return Ok(documentMedical);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Consultation/Document
        [HttpPost("Document")]
        public async Task<ActionResult> UploadDocumentMedicalAsync([FromBody] DocumentMedicalDTO documentMedicalDto)
        {
            try
            {
                if (documentMedicalDto == null)
                {
                    return BadRequest("Invalid document data");
                }

                await _consultationService.UploadDocumentMedicalAsync(documentMedicalDto);
                return Ok("Document Médical uploaded avec succés");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Consultation/Document/{id}
        [HttpDelete("Document/{id}")]
        public async Task<ActionResult> DeleteDocumentMedicalAsync(Guid id)
        {
            try
            {
                var result = await _consultationService.DeleteDocumentMedicalAsync(id);
                if (!result)
                {
                    return NotFound("Document médical not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
