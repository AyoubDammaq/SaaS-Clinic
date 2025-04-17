using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.API.Controllers
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
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> AjouterMedecin([FromBody] MedecinDto medecinDto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de l'ajout du médecin", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenirMedecinParId(Guid id)
        {
            try
            {
                var medecin = await _medecinService.GetDoctorById(id);
                if (medecin == null)
                {
                    return NotFound(new { Message = "Médecin non trouvé" });
                }
                return Ok(medecin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de la récupération du médecin", Details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenirTousLesMedecins()
        {
            try
            {
                var medecins = await _medecinService.GetAllDoctors();
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de la récupération des médecins", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Doctor")]
        public async Task<IActionResult> MettreAJourMedecin(Guid id, [FromBody] MedecinDto medecinDto)
        {
            try
            {
                var medecin = await _medecinService.GetDoctorById(id);
                if (medecin == null)
                {
                    return NotFound(new { Message = "Médecin non trouvé" });
                }
                await _medecinService.UpdateDoctor(id, medecinDto);
                return Ok(new { Message = "Médecin mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de la mise à jour du médecin", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SupprimerMedecin(Guid id)
        {
            try
            {
                var medecin = await _medecinService.GetDoctorById(id);
                if (medecin == null)
                {
                    return NotFound(new { Message = "Médecin non trouvé" });
                }
                await _medecinService.DeleteDoctor(id);
                return Ok(new { Message = "Médecin supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de la suppression du médecin", Details = ex.Message });
            }
        }

        [HttpGet("filter/specialite")]
        public async Task<IActionResult> FiltrerMedecinsBySpecialite([FromQuery] string specialite)
        {
            try
            {
                var medecins = await _medecinService.FilterDoctorsBySpecialite(specialite);
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors du filtrage des médecins par spécialité", Details = ex.Message });
            }
        }

        [HttpGet("filter/name")]
        public async Task<IActionResult> FiltrerMedecinsByName([FromQuery] string? name, [FromQuery] string? prenom)
        {
            try
            {
                var medecins = await _medecinService.FilterDoctorsByName(name ?? string.Empty, prenom ?? string.Empty);
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors du filtrage des médecins par nom", Details = ex.Message });
            }
        }

        [HttpGet("clinique/{cliniqueId}")]
        public async Task<IActionResult> ObtenirMedecinParClinique(Guid cliniqueId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var clinicServiceUrl = $"http://localhost:5291/api/Clinique/{cliniqueId}";
                    var response = await httpClient.GetAsync(clinicServiceUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound(new { Message = "Clinique non trouvée" });
                    }
                }

                var medecins = await _medecinService.GetMedecinByClinique(cliniqueId);
                if (medecins == null || !medecins.Any())
                {
                    return NotFound(new { Message = "Aucun médecin trouvé pour cette clinique" });
                }
                return Ok(medecins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de la récupération des médecins par clinique", Details = ex.Message });
            }
        }

        [HttpPost("attribuer")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        public async Task<IActionResult> AttribuerMedecinAUneClinique([FromBody] AttribuerMedecinDto attribuerMedecinDto)
        {
            try
            {
                var medecin = await _medecinService.GetDoctorById(attribuerMedecinDto.MedecinId);
                if (medecin == null)
                {
                    return NotFound(new { Message = "Médecin non trouvé" });
                }

                using (var httpClient = new HttpClient())
                {
                    var clinicServiceUrl = $"http://localhost:5291/api/Clinique/{attribuerMedecinDto.CliniqueId}";
                    var response = await httpClient.GetAsync(clinicServiceUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound(new { Message = "Clinique non trouvée" });
                    }
                }

                await _medecinService.AttribuerMedecinAUneClinique(attribuerMedecinDto.MedecinId, attribuerMedecinDto.CliniqueId);
                return Ok(new { Message = "Médecin attribué à la clinique avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors de l'attribution du médecin à la clinique", Details = ex.Message });
            }
        }

        [HttpDelete("desabonner/{medecinId}")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        public async Task<IActionResult> DesabonnerMedecinDeClinique(Guid medecinId)
        {
            try
            {
                var medecin = await _medecinService.GetDoctorById(medecinId);
                if (medecin == null)
                {
                    return NotFound(new { Message = "Médecin non trouvé" });
                }
                await _medecinService.DesabonnerMedecinDeClinique(medecinId);
                return Ok(new { Message = "Médecin désabonné de la clinique avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue lors du désabonnement du médecin de la clinique", Details = ex.Message });
            }
        }
    }
}
