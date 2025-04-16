using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementService.DTOs;
using PatientManagementService.Models;
using PatientManagementService.Services;

namespace PatientManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: api/Patients
        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
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
        public async Task<ActionResult<Patient>> GetPatientById(Guid id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
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
                await _patientService.AddPatientAsync(patientDto);
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

                var existingPatient = await _patientService.GetPatientByIdAsync(id);
                if (existingPatient == null)
                    return NotFound("Patient non trouvé.");

                await _patientService.UpdatePatientAsync(patientDto);
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
                var existingPatient = await _patientService.GetPatientByIdAsync(id);
                if (existingPatient == null)
                    return NotFound("Patient non trouvé.");

                await _patientService.DeletePatientAsync(id);
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
                var patients = await _patientService.GetPatientsByNameAsync(name, lastname);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
    }
}
