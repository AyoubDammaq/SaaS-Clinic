using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Application.PatientService.Commands.DeletePatient;
using PatientManagementService.Application.PatientService.Commands.UpdatePatient;
using PatientManagementService.Application.PatientService.Queries.GetAllPatients;
using PatientManagementService.Application.PatientService.Queries.GetPatientById;
using PatientManagementService.Application.PatientService.Queries.GetPatientsByName;
using PatientManagementService.Application.PatientService.Queries.GetStatistiques;

namespace PatientManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;   

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // GET: api/Patients
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Patient>>> GetAllPatients()
        {
            try
            {
                var patients = await _mediator.Send(new GetAllPatientsQuery());
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // GET: api/Patients/5
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Entities.Patient>> GetPatientById(Guid id)
        {
            try
            {
                var patient = await _mediator.Send(new GetPatientByIdQuery(id));
                if (patient == null)
                    return NotFound("Patient non trouvé.");

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // POST: api/Patients
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] PatientDTO patientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                patientDto.Id = Guid.NewGuid();
                await _mediator.Send(new AddPatientCommand(patientDto));
                return CreatedAtAction(nameof(GetPatientById), new { id = patientDto.Id }, patientDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // PUT: api/Patients/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(Guid id, [FromBody] PatientDTO patientDto)
        {
            try
            {
                if (id != patientDto.Id)
                    return BadRequest("L'ID fourni ne correspond pas à celui du patient.");

                var existingPatient = await _mediator.Send(new GetPatientByIdQuery(id));
                if (existingPatient == null)
                    return NotFound("Patient non trouvé.");

                await _mediator.Send(new UpdatePatientCommand(patientDto));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // DELETE: api/Patients/5
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(Guid id)
        {
            try
            {
                var existingPatient = await _mediator.Send(new GetPatientByIdQuery(id));
                if (existingPatient == null)
                    return NotFound("Patient non trouvé.");

                await _mediator.Send(new DeletePatientCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // GET: api/Patients/search?name=John&lastname=Doe
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Patient>>> SearchPatients([FromQuery] string? name, [FromQuery] string? lastname)
        {
            try
            {
                var patients = await _mediator.Send(new GetPatientsByNameQuery(name, lastname));
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("statistiques")]
        public async Task<ActionResult<int>> GetStatistiques([FromQuery] DateTime dateDebut, [FromQuery] DateTime dateFin)
        {
            if (dateDebut > dateFin)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            var result = await _mediator.Send(new GetStatistiquesQuery(dateDebut, dateFin));
            return Ok(result);
        }
    }
}
