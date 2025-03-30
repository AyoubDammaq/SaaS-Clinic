using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RessourceManagementService.Data;
using RessourceManagementService.Models;

namespace RessourceManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RessourceMedicaleController : ControllerBase
    {
        private readonly RessourceMedicaleDbContext _context;

        public RessourceMedicaleController(RessourceMedicaleDbContext context)
        {
            _context = context;
        }

        // GET: api/ressources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RessourceMedicale>>> GetRessources()
        {
            return await _context.RessourcesMedicales.ToListAsync();
        }

        // GET: api/ressources/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RessourceMedicale>> GetRessource(int id)
        {
            var ressource = await _context.RessourcesMedicales.FindAsync(id);
            if (ressource == null)
                return NotFound();

            return ressource;
        }

        // POST: api/ressources
        [HttpPost]
        public async Task<ActionResult<RessourceMedicale>> CreateRessource(RessourceMedicale ressource)
        {
            _context.RessourcesMedicales.Add(ressource);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRessource), new { id = ressource.Id }, ressource);
        }
    }
}
