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
        private readonly IDisponibiliteService _disponibiliteService;
        public MedecinController(IMedecinService medecinService, IDisponibiliteService disponibiliteService)
        {
            _medecinService = medecinService;
            _disponibiliteService = disponibiliteService;
        }

        [HttpPost]
        public async Task<IActionResult> AjouterMedecin([FromBody] MedecinDto medecinDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (medecinDto.CliniqueId == Guid.Empty)
            {
                medecinDto.CliniqueId = null;
            }

            var medecin = new Medecin
            {
                Prenom = medecinDto.Prenom,
                Nom = medecinDto.Nom,
                Specialite = medecinDto.Specialite,
                Email = medecinDto.Email,
                Telephone = medecinDto.Telephone,
                CliniqueId = medecinDto.CliniqueId ?? null,
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
        public async Task<IActionResult> FiltrerMedecinsByName([FromQuery] string? name, [FromQuery] string? prenom)
        {
            // Correction des problèmes CS8604 en utilisant des valeurs par défaut pour les paramètres null
            var medecins = await _medecinService.FilterDoctorsByName(name ?? string.Empty, prenom ?? string.Empty);
            return Ok(medecins);
        }

        [HttpGet("clinique/{cliniqueId}")]
        public async Task<IActionResult> ObtenirMedecinParClinique(Guid cliniqueId)
        {
            var medecins = await _medecinService.GetMedecinByClinique(cliniqueId);
            if (medecins == null || !medecins.Any())
            {
                return NotFound(new { Message = "Aucun médecin trouvé pour cette clinique" });
            }
            return Ok(medecins);
        }

        [HttpPost("attribuer")]
        public async Task<IActionResult> AttribuerMedecinAUneClinique([FromBody] AttribuerMedecinDto attribuerMedecinDto)
        {
            var medecin = await _medecinService.GetDoctorById(attribuerMedecinDto.MedecinId);
            if (medecin == null)
            {
                return NotFound(new { Message = "Médecin non trouvé" });
            }
            await _medecinService.AttribuerMedecinAUneClinique(attribuerMedecinDto.MedecinId, attribuerMedecinDto.CliniqueId);
            return Ok(new { Message = "Médecin attribué à la clinique avec succès" });
        }

        [HttpDelete("desabonner/{medecinId}")]
        public async Task<IActionResult> DesabonnerMedecinDeClinique(Guid medecinId)
        {
            var medecin = await _medecinService.GetDoctorById(medecinId);
            if (medecin == null)
            {
                return NotFound(new { Message = "Médecin non trouvé" });
            }
            await _medecinService.DesabonnerMedecinDeClinique(medecinId);
            return Ok(new { Message = "Médecin désabonné de la clinique avec succès" });
        }
    }
}