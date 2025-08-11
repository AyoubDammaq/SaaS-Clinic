using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Application.PatientService.Commands.DeletePatient;
using PatientManagementService.Application.PatientService.Commands.LinkUserToPatient;
using PatientManagementService.Application.PatientService.Commands.UpdatePatient;
using PatientManagementService.Application.PatientService.Queries.CountTotalPatients;
using PatientManagementService.Application.PatientService.Queries.GetAllPatients;
using PatientManagementService.Application.PatientService.Queries.GetNombreDeNouveauxPatientsParMois;
using PatientManagementService.Application.PatientService.Queries.GetPatientById;
using PatientManagementService.Application.PatientService.Queries.GetPatientsByName;
using PatientManagementService.Application.PatientService.Queries.GetStatistiques;
using PatientManagementService.Domain.Entities;

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
        [HttpGet]
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor,Patient"))]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
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
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(Guid id)
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
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] CreatePatientDTO patientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdPatient = await _mediator.Send(new AddPatientCommand(patientDto));
                return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        // PUT: api/Patients/5
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor, Patient")]
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
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string? name, [FromQuery] string? lastname)
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

        [HttpGet("count/total-patients")]
        public async Task<ActionResult<int>> CountTotalPatients()
        {
            try
            {
                var totalPatients = await _mediator.Send(new CountTotalPatientsQuery());
                return Ok(totalPatients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpGet("nouveaux-patients/mois")]
        public async Task<ActionResult<Dictionary<string, int>>> GetNombreDeNouveauxPatientsParMois([FromQuery] DateTime dateActuel)
        {
            try
            {
                var result = await _mediator.Send(new GetNombreDeNouveauxPatientsParMoisQuery(dateActuel));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor, Patient"))]
        [HttpPost("link-user/patient")]
        public async Task<ActionResult> LinkUserToPatient([FromBody] LinkDto linkDto)
        {
            try
            {
                var result = await _mediator.Send(new LinkUserToPatientCommand(linkDto));
                if (result)
                    return Ok("Utilisateur lié au patient avec succès.");
                else
                    return BadRequest("Échec de la liaison de l'utilisateur au patient.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
    }
}
