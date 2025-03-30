using FacturationService.Data;
using FacturationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacturationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly FacturationDbContext _context;

        public FactureController(FacturationDbContext context)
        {
            _context = context;
        }

        // GET: api/factures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facture>>> GetFactures()
        {
            return await _context.Factures.ToListAsync();
        }

        // GET: api/factures/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Facture>> GetFacture(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
                return NotFound();

            return facture;
        }

        // POST: api/factures
        [HttpPost]
        public async Task<ActionResult<Facture>> CreateFacture(Facture facture)
        {
            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFacture), new { id = facture.Id }, facture);
        }
    }
}
