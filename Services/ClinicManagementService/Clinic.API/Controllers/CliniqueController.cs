using Clinic.Application.Commands.AjouterClinique;
using Clinic.Application.Commands.LinkUserToClinic;
using Clinic.Application.Commands.ModifierClinique;
using Clinic.Application.Commands.SupprimerClinique;
using Clinic.Application.DTOs;
using Clinic.Application.Queries.GetNombreCliniques;
using Clinic.Application.Queries.GetNombreDeCliniquesParDate;
using Clinic.Application.Queries.GetNombreNouvellesCliniquesDuMois;
using Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois;
using Clinic.Application.Queries.GetStatistiquesDesCliniques;
using Clinic.Application.Queries.ListerClinique;
using Clinic.Application.Queries.ObtenirCliniqueParId;
using Clinic.Application.Queries.RechercherCliniqueParAdresse;
using Clinic.Application.Queries.RechercherCliniqueParNom;
using Clinic.Application.Queries.RechercherCliniqueParStatut;
using Clinic.Application.Queries.RechercherCliniqueParType;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CliniqueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CliniqueController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
                var result = await _mediator.Send(new AjouterCliniqueCommand(clinique));
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
                await _mediator.Send(new UpdateCliniqueCommand(id, clinique));
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
                var result = await _mediator.Send(new SupprimerCliniqueCommand(id));
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
                var clinique = await _mediator.Send(new ObtenirCliniqueParIdQuery(id));
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
                var cliniques = await _mediator.Send(new ListerCliniquesQuery());
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
                var cliniques = await _mediator.Send(new RechercherCliniqueParNomQuery(nom));
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
                var cliniques = await _mediator.Send(new RechercherCliniqueParAdresseQuery(adresse));
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
                var cliniques = await _mediator.Send(new RechercherCliniqueParTypeQuery(type));
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
                var cliniques = await _mediator.Send(new RechercherCliniqueParStatusQuery(statut));
                return Ok(cliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par statut : {ex.Message}");
            }
        }


        // Statistiques des cliniques
        [AllowAnonymous]
        [HttpGet("nombre-cliniques")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetNombreDeCliniques()
        {
            try
            {
                var nombreCliniques = await _mediator.Send(new GetNombreCliniquesQuery());
                return Ok(nombreCliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération du nombre des cliniques : {ex.Message}");
            }

        }

        [AllowAnonymous]
        [HttpGet("nombre-cliniques-par-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetNombreDeCliniquesParDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var nombreCliniques = await _mediator.Send(new GetNombreDeCliniquesParDateQuery(startDate, endDate));
                return Ok(nombreCliniques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération du nombre des cliniques par date : {ex.Message}");
            }
        }

        [HttpGet("nouvelles-cliniques-mois")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetNombreNouvellesCliniquesDuMois()
        {
            try
            {
                return Ok(await _mediator.Send(new GetNombreNouvellesCliniquesDuMoisQuery()));
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
                return Ok(await _mediator.Send(new GetNombreNouvellesCliniquesParMoisQuery()));
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
                var statistiques = await _mediator.Send(new GetStatistiquesDesCliniquesQuery(cliniqueId));
                return statistiques == null ? NotFound($"Aucune statistique trouvée pour la clinique avec l'ID {cliniqueId}.") : Ok(statistiques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des statistiques de la clinique : {ex.Message}");
            }
        }

        [Authorize(Roles = ("SuperAdmin, ClinicAdmin, Doctor"))]
        [HttpPost("link-user/clinic")]
        public async Task<IActionResult> LinkUserToClinicEntity(LinkDTO linkDTO)
        {
            try
            {
                var result = await _mediator.Send(new LinkUserToClinicCommand(linkDTO));
                return result ? Ok() : NotFound($"Clinique avec l'ID {linkDTO.ClinicId} ou utilisateur avec l'ID {linkDTO.UserId} introuvable.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la liaison de l'utilisateur à la clinique : {ex.Message}");
            }
        }
    }
}



