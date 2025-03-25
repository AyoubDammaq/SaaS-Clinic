using ClinicManagementService.Data;
using ClinicManagementService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CliniqueController(CliniqueDbContext _context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCliniques() =>
            Ok(await _context.Cliniques.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> CreateClinique(Clinique clinique)
        {
            _context.Cliniques.Add(clinique);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCliniques), new { id = clinique.Id }, clinique);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliniqueById(Guid id)
        {
            var clinique = await _context.Cliniques.FindAsync(id);
            if (clinique == null)
            {
                return NotFound($"Clinique with ID {id} not found.");
            }
            return Ok(clinique);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinique(Guid id, Clinique updatedClinique)
        {
            if (updatedClinique == null || id != updatedClinique.Id)
            {
                return BadRequest("Invalid clinique data or ID mismatch.");
            }

            var existingClinique = await _context.Cliniques.FindAsync(id);
            if (existingClinique == null)
            {
                return NotFound($"Clinique with ID {id} not found.");
            }

            // Update the existing clinique with new values
            existingClinique.Nom = updatedClinique.Nom;
            existingClinique.Adresse = updatedClinique.Adresse;
            existingClinique.Ville = updatedClinique.Ville;
            existingClinique.Pays = updatedClinique.Pays;
            existingClinique.CodePostal = updatedClinique.CodePostal;
            existingClinique.Téléphone = updatedClinique.Téléphone;
            existingClinique.Email = updatedClinique.Email;

            _context.Cliniques.Update(existingClinique);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinique(Guid id)
        {
            var clinique = await _context.Cliniques.FindAsync(id);
            if (clinique == null)
            {
                return NotFound($"Clinique with ID {id} not found.");
            }

            _context.Cliniques.Remove(clinique);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
