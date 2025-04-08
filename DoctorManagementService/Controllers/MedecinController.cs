using DoctorManagementService.DTOs;
using DoctorManagementService.Models;
using DoctorManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedecinController : ControllerBase
    {
        private readonly IMedecinService _medecinService;
        public MedecinController(IMedecinService medecinService)
        {
            _medecinService = medecinService;
        }

        [HttpPost]
        public async Task<IActionResult> AjouterMedecin([FromBody] MedecinDto medecinDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var medecin = new Medecin
            {
                Prenom = medecinDto.Prenom,
                Nom = medecinDto.Nom,
                Specialite = medecinDto.Specialite,
                Email = medecinDto.Email,
                Telephone = medecinDto.Telephone,
                CliniqueId = medecinDto.CliniqueId ?? Guid.Empty,
                PhotoUrl = medecinDto.PhotoUrl,
                DateCreation = DateTime.UtcNow
            };

            await _medecinService.AddDoctor(medecin);
            return Ok(new { Message = "Médecin ajouté avec succès" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenirMedecinParId(Guid id)
        {
            var medecin = await _medecinService.GetDoctorById(id);
            if (medecin == null)
            {
                return NotFound(new { Message = "Médecin non trouvé" });
            }
            return Ok(medecin);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenirTousLesMedecins()
        {
            var medecins = await _medecinService.GetAllDoctors();
            return Ok(medecins);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MettreAJourMedecin(Guid id, [FromBody] MedecinDto medecinDto)
        {
            var medecin = await _medecinService.GetDoctorById(id);
            if (medecin == null)
            {
                return NotFound(new { Message = "Médecin non trouvé" });
            }
            await _medecinService.UpdateDoctor(id, medecinDto);
            return Ok(new { Message = "Médecin mis à jour avec succès" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerMedecin(Guid id)
        {
            var medecin = await _medecinService.GetDoctorById(id);
            if (medecin == null)
            {
                return NotFound(new { Message = "Médecin non trouvé" });
            }
            await _medecinService.DeleteDoctor(id);
            return Ok(new { Message = "Médecin supprimé avec succès" });
        }

        [HttpGet("filter/specialite")]
        public async Task<IActionResult> FiltrerMedecinsBySpecialite([FromQuery] string specialite)
        {
            var medecins = await _medecinService.FilterDoctorsBySpecialite(specialite);
            return Ok(medecins);
        }

        [HttpGet("filter/name")]
        public async Task<IActionResult> FiltrerMedecinsByName([FromQuery] string? name, [FromQuery]  string? prenom)
        {
            var medecins = await _medecinService.FilterDoctorsByName(name, prenom);
            return Ok(medecins);
        }



    }
}
