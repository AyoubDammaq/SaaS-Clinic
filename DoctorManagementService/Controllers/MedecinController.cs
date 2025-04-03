using DoctorManagementService.DTOs;
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
            await _medecinService.AddDoctor(medecinDto);
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
        [HttpGet("filter")]
        public async Task<IActionResult> FiltrerMedecins([FromQuery] string specialite)
        {
            var medecins = await _medecinService.FilterDoctors(specialite);
            return Ok(medecins);
        }

    }
}
