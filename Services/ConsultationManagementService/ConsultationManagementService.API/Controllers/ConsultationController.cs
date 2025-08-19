using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ConsultationManagementService.Application.Queries.GetConsultationById;
using ConsultationManagementService.Application.Queries.GetAllConsultations;
using ConsultationManagementService.Application.Commands.CreateConsultation;
using ConsultationManagementService.Application.Commands.UpdateConsultation;
using ConsultationManagementService.Application.Commands.DeleteConsultation;
using ConsultationManagementService.Application.Queries.GetConsultationsByPatientId;
using ConsultationManagementService.Application.Queries.GetConsultationsByDoctorId;
using ConsultationManagementService.Application.Queries.GetDocumentMedicalById;
using ConsultationManagementService.Application.Commands.UploadDocumentMedical;
using ConsultationManagementService.Application.Commands.DeleteDocumentMedical;
using ConsultationManagementService.Application.Queries.GetNombreConsultations;
using ConsultationManagementService.Application.Queries.CountByMedecinIds;
using ConsultationManagementService.Application.Queries.GetConsultationsByClinicId;
using ConsultationManagementService.Application.Queries.CountNouveauxPatientsByDoctor;
using ConsultationManagementService.Application.Queries.CountNouveauxPatientsByClinic;
using ConsultationManagementService.Application.Queries.CountConsultationByDate;
using ConsultationManagementService.Application.Queries.CountConsultationByPatient;
using ConsultationManagementService.Application.Queries.CountConsultationByDoctor;
using ConsultationManagementService.Application.Queries.CountConsultationByClinic;

namespace ConsultationManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConsultationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Consultation/{id}
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
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

                var consultation = await _mediator.Send(new GetConsultationByIdQuery(id));

                return Ok(consultation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Consultation>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Consultation>>> GetAllConsultationsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var consultations = await _mediator.Send(new GetAllConsultationsQuery(pageNumber, pageSize));
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

                await _mediator.Send(new CreateConsultationCommand(consultationDto));
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

                await _mediator.Send(new UpdateConsultationCommand(consultationDto));
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

                var result = await _mediator.Send(new DeleteConsultationCommand(id));
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
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
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

                var consultations = await _mediator.Send(new GetConsultationsByPatientIdQuery(patientId));

                return Ok(consultations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation/Doctor/{doctorId}
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
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

                var consultations = await _mediator.Send(new GetConsultationsByDoctorIdQuery(doctorId));

                return Ok(consultations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Consultation/Document/{id}
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
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

                var documentMedical = await _mediator.Send(new GetDocumentMedicalByIdQuery(id));

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
        public async Task<IActionResult> UploadDocumentMedicalAsync([FromForm] Guid consultationId, [FromForm] string type, [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Aucun fichier reçu.");
                }

                // Générer un chemin de stockage (ici temporaire local pour test)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); // Crée le dossier si nécessaire

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fichierUrl = $"/uploads/{uniqueFileName}";

                var dto = new DocumentMedicalDTO
                {
                    Id = Guid.NewGuid(),
                    ConsultationId = consultationId,
                    FileName = file.FileName,
                    Type = type,
                    FichierURL = fichierUrl,
                    DateAjout = DateTime.Now
                };

                await _mediator.Send(new UploadDocumentMedicalCommand(dto));
                return Ok("Document Médical uploadé avec succès");
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

                var result = await _mediator.Send(new DeleteDocumentMedicalCommand(id));
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

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("count")]
        public async Task<IActionResult> GetNombreConsultations([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var count = await _mediator.Send(new GetNombreConsultationsQuery(start, end));
            return Ok(count);
        }

        //[Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("countByMedecinIds")]
        public async Task<IActionResult> GetCountByMedecinIds([FromQuery] List<Guid> medecinIds)
        {
            if (medecinIds == null || !medecinIds.Any())
            {
                return BadRequest("La liste des identifiants de médecins ne peut pas être vide.");
            }

            var count = await _mediator.Send(new CountByMedecinIdsQuery(medecinIds));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("by-clinic")]
        public async Task<IActionResult> GetConsultationsByClinicId([FromQuery] Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Invalid ClinicId");

            var consultations = await _mediator.Send(new GetConsultationsByClinicIdQuery(clinicId));
            return Ok(consultations);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpGet("count-by-date")]
        public async Task<IActionResult> CountConsultation(
            [FromQuery] DateTime dateDebut,
            [FromQuery] DateTime dateFin,
            [FromQuery] Guid? cliniqueId,
            [FromQuery] Guid? medecinId,
            [FromQuery] Guid? patientId)
        {
            if (dateDebut >= dateFin)
                return BadRequest("Start date must be earlier than end date");

            var count = await _mediator.Send(new CountConsultationByDateQuery(cliniqueId, medecinId, patientId, dateDebut, dateFin));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("nouveaux-patients-count-by-doctor/{medecinId}")]
        public async Task<IActionResult> GetNouveauxPatientsCount(Guid medecinId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (medecinId == Guid.Empty)
                return BadRequest("Invalid MedecinId");
            if (startDate >= endDate)
                return BadRequest("Start date must be earlier than end date");
            var count = await _mediator.Send(new CountNouveauxPatientsByDoctorQuery(medecinId, startDate, endDate));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        [HttpGet("nouveaux-patients-count-by-clinic/{clinicId}")]
        public async Task<IActionResult> GetNouveauxPatientsCountByClinic(Guid clinicId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Invalid clinicId");
            if (startDate >= endDate)
                return BadRequest("Start date must be earlier than end date");
            var count = await _mediator.Send(new CountNouveauxPatientsByClinicQuery(clinicId, startDate, endDate));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpGet("count-consultation-by-patient/{patientId}")]
        public async Task<IActionResult> CountConsultationByPatient(Guid patientId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (patientId == Guid.Empty)
                return BadRequest("Invalid PatientId");
            var count = await _mediator.Send(new CountConsultationByPatientQuery(patientId, startDate, endDate));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpGet("count-consultation-by-doctor/{medecinId}")]
        public async Task<IActionResult> CountConsultationByDoctor(Guid medecinId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (medecinId == Guid.Empty)
                return BadRequest("Invalid MedecinId");
            var count = await _mediator.Send(new CountConsultationByDoctorQuery(medecinId, startDate, endDate));
            return Ok(count);
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpGet("count-consultation-by-clinic/{clinicId}")]
        public async Task<IActionResult> CountConsultationByClinic(Guid clinicId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Invalid ClinicId");
            var count = await _mediator.Send(new CountConsultationByClinicQuery(clinicId, startDate, endDate));
            return Ok(count);
        }

        [HttpGet("count-consultation-by-clinic-per-month/{clinicId}")]
        public async Task<IActionResult> CountConsultationByClinicPerMonth(Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Invalid ClinicId");

            var currentYear = DateTime.UtcNow.Year;
            var monthlyCounts = new Dictionary<int, int>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(currentYear, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                int count = 0;
                try
                {
                    count = await _mediator.Send(
                        new CountConsultationByClinicQuery(clinicId, startDate, endDate)
                    );
                }
                catch
                {
                    count = 0; // Toujours 0 si erreur
                }

                monthlyCounts[month] = count;
            }

            return Ok(monthlyCounts);
        }

        [HttpGet("nouveaux-patients-count-by-clinic-per-month/{clinicId}")]
        public async Task<IActionResult> GetNouveauxPatientsCountByClinicPerMonth(Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Invalid clinicId");

            var currentYear = DateTime.UtcNow.Year;
            var monthlyCounts = new Dictionary<int, int>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(currentYear, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                int count = 0;
                try
                {
                    count = await _mediator.Send(
                        new CountNouveauxPatientsByClinicQuery(clinicId, startDate, endDate)
                    );
                }
                catch
                {
                    count = 0; // Toujours 0 si erreur
                }

                monthlyCounts[month] = count;
            }

            return Ok(monthlyCounts);
        }
    }
}
