using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManagementService.Data;
using PatientManagementService.Models;

namespace PatientManagementService.Controllers
{
    public class PatientController : Controller
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PatientsController : ControllerBase
        {
            private readonly PatientDbContext _context;

            public PatientsController(PatientDbContext context)
            {
                _context = context;
            }

            // GET: api/patients
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
            {
                return await _context.Patients.ToListAsync();
            }

            // GET: api/patients/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<Patient>> GetPatient(Guid id)
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound();

                return patient;
            }

            // POST: api/patients
            [HttpPost]
            public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }

            // PUT: api/patients/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdatePatient(Guid id, Patient patient)
            {
                if (id != patient.Id)
                    return BadRequest();

                _context.Entry(patient).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/patients/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePatient(Guid id)
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                    return NotFound();

                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
