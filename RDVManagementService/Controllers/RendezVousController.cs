using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDVManagementService.Data;
using RDVManagementService.Models;

namespace RDVManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RendezVousController : ControllerBase
    {
        private readonly RendezVousDbContext _context;

        public RendezVousController(RendezVousDbContext context)
        {
            _context = context;
        }

        // GET: api/rendezvous
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVous()
        {
            return await _context.RendezVous.ToListAsync();
        }

        // GET: api/rendezvous/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RendezVous>> GetRendezVous(int id)
        {
            var rendezVous = await _context.RendezVous.FindAsync(id);
            if (rendezVous == null)
                return NotFound();

            return rendezVous;
        }

        // POST: api/rendezvous
        [HttpPost]
        public async Task<ActionResult<RendezVous>> CreateRendezVous(RendezVous rendezVous)
        {
            _context.RendezVous.Add(rendezVous);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRendezVous), new { id = rendezVous.Id }, rendezVous);
        }

        // PUT: api/rendezvous/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRendezVous(Guid id, RendezVous rendezVous)
        {
            if (id != rendezVous.Id)
                return BadRequest();

            _context.Entry(rendezVous).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/rendezvous/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRendezVous(Guid id)
        {
            var rendezVous = await _context.RendezVous.FindAsync(id);
            if (rendezVous == null)
                return NotFound();

            _context.RendezVous.Remove(rendezVous);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
