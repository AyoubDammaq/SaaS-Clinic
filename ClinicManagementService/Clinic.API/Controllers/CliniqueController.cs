﻿using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CliniqueController : ControllerBase
    {
        private readonly ICliniqueService _cliniqueService;

        public CliniqueController(ICliniqueService cliniqueService)
        {
            _cliniqueService = cliniqueService;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        public async Task<ActionResult<Clinique>> AjouterClinique(Clinique clinique)
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
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniques()
        {
            try
            {
                return Ok(await _cliniqueService.ListerCliniqueAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques : {ex.Message}");
            }
        }

        [HttpGet("nom/{nom}")]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParNom(string nom)
        {
            try
            {
                return Ok(await _cliniqueService.ListerCliniquesParNomAsync(nom));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par nom : {ex.Message}");
            }
        }

        [HttpGet("adresse/{adresse}")]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniquesParAdresse(string adresse)
        {
            try
            {
                return Ok(await _cliniqueService.ListerCliniquesParAdresseAsync(adresse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des cliniques par adresse : {ex.Message}");
            }
        }
    }
}
