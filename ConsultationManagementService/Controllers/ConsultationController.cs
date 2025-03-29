using ConsultationManagementService.Data;
using ConsultationManagementService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly ConsultationDbContext _context;

        public ConsultationController(ConsultationDbContext context)
        {
            _context = context;
        }

        // GET: api/consultation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consultation>>> GetConsultations()
        {
            return await _context.Consultations.Include(c => c.Documents).ToListAsync();
        }

        // GET: api/consultation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Consultation>> GetConsultation(Guid id)
        {
            var consultation = await _context.Consultations.Include(c => c.Documents)
                                                           .FirstOrDefaultAsync(c => c.Id == id);
            if (consultation == null)
                return NotFound();

            return consultation;
        }

        // POST: api/consultation
        [HttpPost]
        public async Task<ActionResult<Consultation>> CreateConsultation(Consultation consultation)
        {
            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetConsultation), new { id = consultation.Id }, consultation);
        }
    }
}
