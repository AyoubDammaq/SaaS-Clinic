using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultationManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService ?? throw new ArgumentNullException(nameof(consultationService));
        }

        // GET: api/Consultation/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Consultation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Consultation>> GetConsultationByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid consultation ID");
                }

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Consultation>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateConsultationAsync([FromBody] ConsultationDTO consultationDto)
        {
            try
            {
                if (consultationDto == null)
                {
                    return BadRequest("Invalid consultation data");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _consultationService.CreateConsultationAsync(consultationDto);
                return Ok("Consultation créée avec succès");
            }
            catch (ArgumentException ex) // Ajout de la gestion spécifique des erreurs de validation
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Consultation
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateConsultationAsync([FromBody] ConsultationDTO consultationDto)
        {
            try
            {
                if (consultationDto == null)
                {
                    return BadRequest("Invalid consultation data");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _consultationService.UpdateConsultationAsync(consultationDto);
                return NoContent();
            }
            catch (ArgumentException ex) // Ajout de la gestion spécifique des erreurs de validation
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Ajout de la gestion des erreurs de consultation non trouvée
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Consultation/{id}
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteConsultationAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid consultation ID");
                }

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

        // GET: api/Consultation/Patient/{patientId}
        [HttpGet("Patient/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Consultation>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Consultation>>> GetConsultationsByPatientIdAsync(Guid patientId)
        {
            try
            {
                if (patientId == Guid.Empty)
                {
                    return BadRequest("Invalid patient ID");
                }

                var consultations = await _consultationService.GetConsultationsByPatientIdAsync(patientId);
                if (!consultations.Any())
                {
                    return NotFound($"No consultations found for patient with ID {patientId}");
                }

                return Ok(consultations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation/Doctor/{doctorId}
        [HttpGet("Doctor/{doctorId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Consultation>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Consultation>>> GetConsultationsByDoctorIdAsync(Guid doctorId)
        {
            try
            {
                if (doctorId == Guid.Empty)
                {
                    return BadRequest("Invalid doctor ID");
                }

                var consultations = await _consultationService.GetConsultationsByDoctorIdAsync(doctorId);
                if (!consultations.Any())
                {
                    return NotFound($"No consultations found for doctor with ID {doctorId}");
                }

                return Ok(consultations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
      
        // GET: api/Consultation/Document/{id}
        [HttpGet("Document/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DocumentMedical))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentMedical>> GetDocumentMedicalByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid document ID");
                }

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
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpPost("Document")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UploadDocumentMedicalAsync([FromBody] DocumentMedicalDTO documentMedicalDto)
        {
            try
            {
                if (documentMedicalDto == null)
                {
                    return BadRequest("Invalid document data");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _consultationService.UploadDocumentMedicalAsync(documentMedicalDto);
                return Ok("Document Médical uploadé avec succès");
            }
            catch (ArgumentException ex) // Ajout de la gestion spécifique des erreurs de validation
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Consultation/Document/{id}
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpDelete("Document/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDocumentMedicalAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid document ID");
                }

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

        [HttpGet("count")]
        public async Task<IActionResult> GetNombreConsultations([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var count = await _consultationService.GetNombreConsultationsAsync(start, end);
            return Ok(count);
        }

        [HttpGet("countByMedecinIds")]
        public async Task<IActionResult> GetCountByMedecinIds([FromQuery] List<Guid> medecinIds)
        {
            if (medecinIds == null || !medecinIds.Any())
            {
                return BadRequest("La liste des identifiants de médecins ne peut pas être vide.");
            }
            var count = await _consultationService.CountByMedecinIds(medecinIds);
            return Ok(count);
        }

    }
}
