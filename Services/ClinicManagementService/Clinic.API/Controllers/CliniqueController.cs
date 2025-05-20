using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CliniqueController : ControllerBase
    {
        private readonly ICliniqueService _cliniqueService;

        public CliniqueController(ICliniqueService cliniqueService)
        {
            _cliniqueService = cliniqueService ?? throw new ArgumentNullException(nameof(cliniqueService), "Le service de clinique ne peut pas être null.");
        }

        // CRUD operations
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clinique>> AjouterClinique(CliniqueDto clinique)
        {
            try
            {
                var result = await _cliniqueService.AjouterCliniqueAsync(clinique);
                return CreatedAtAction(nameof(ObtenirClinique), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout de la clinique : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModifierClinique(Guid id, Clinique clinique)
        {
            try
            {
                await _cliniqueService.ModifierCliniqueAsync(id, clinique);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Clinique avec l'ID {id} introuvable.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la modification de la clinique : {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SupprimerClinique(Guid id)
        {
            try
            {
                var result = await _cliniqueService.SupprimerCliniqueAsync(id);
                return result ? NoContent() : NotFound($"Clinique avec l'ID {id} introuvable.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression de la clinique : {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Clinique))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clinique>> ObtenirClinique(Guid id)
        {
            try
            {
                var clinique = await _cliniqueService.ObtenirCliniqueParIdAsync(id);
                return clinique == null ? NotFound($"Clinique avec l'ID {id} introuvable.") : Ok(clinique);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération de la clinique : {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Clinique>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniques()
        {
            try
            {
                var cliniques = await _cliniqueService.ListerCliniqueAsync();
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques : {ex.Message}");
            }
        }

        // Recherche des cliniques
        [HttpGet("nom/{nom}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Clinique>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParNom(string nom)
        {
            try
            {
                var cliniques = await _cliniqueService.ListerCliniquesParNomAsync(nom);
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par nom : {ex.Message}");
            }
        }

        [HttpGet("adresse/{adresse}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Clinique>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParAdresse(string adresse)
        {
            try
            {
                var cliniques = await _cliniqueService.ListerCliniquesParAdresseAsync(adresse);
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par adresse : {ex.Message}");
            }
        }

        [HttpGet("type/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Clinique>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParType(TypeClinique type)
        {
            try
            {
                var cliniques = await _cliniqueService.ListerCliniquesParTypeAsync(type);
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par type : {ex.Message}");
            }
        }

        [HttpGet("statut/{statut}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Clinique>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParStatut(StatutClinique statut)
        {
            try
            {
                var cliniques = await _cliniqueService.ListerCliniquesParStatutAsync(statut);
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par statut : {ex.Message}");
            }
        }


        // Statistiques des cliniques
        [HttpGet("nombre-cliniques")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetNombreDeCliniques()
        {
            try
            {
                var nombreCliniques = await _cliniqueService.GetNombreCliniques();
                return Ok(nombreCliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération du nombre des cliniques : {ex.Message}");
            }

        }

        [HttpGet("nouvelles-cliniques-mois")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetNombreNouvellesCliniquesDuMois()
        {
            try
            {
                return Ok(await _cliniqueService.GetNombreNouvellesCliniquesDuMois());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des nouvelles cliniques du dernier mois: {ex.Message}");
            }
        }
        [HttpGet("nouvelles-cliniques-par-mois")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StatistiqueDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StatistiqueDTO>>> GetNombreNouvellesCliniquesParMois()
        {
            try
            {
                return Ok(await _cliniqueService.GetNombreNouvellesCliniquesParMois());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des nouvelles cliniques par mois: {ex.Message}");
            }
        }

        [HttpGet("statistiques/{cliniqueId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StatistiqueCliniqueDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatistiqueCliniqueDTO>> GetStatistiquesDesCliniques(Guid cliniqueId)
        {
            try
            {
                var statistiques = await _cliniqueService.GetStatistiquesDesCliniquesAsync(cliniqueId);
                return statistiques == null ? NotFound($"Aucune statistique trouvée pour la clinique avec l'ID {cliniqueId}.") : Ok(statistiques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des statistiques de la clinique : {ex.Message}");
            }
        }
    }
}

