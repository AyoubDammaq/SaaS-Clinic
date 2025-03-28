﻿using ClinicManagementService.Data;
using ClinicManagementService.Models;
using ClinicManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CliniqueController() : ControllerBase
    {
        private readonly ICliniqueService _cliniqueService;

        public CliniqueController(ICliniqueService cliniqueService) : this()
        {
            _cliniqueService = cliniqueService;
        }

        [HttpPost]
        public async Task<ActionResult<Clinique>> AjouterClinique(Clinique clinique)
        {
            var result = await _cliniqueService.AjouterCliniqueAsync(clinique);
            return CreatedAtAction(nameof(ObtenirClinique), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierClinique(Guid id, Clinique clinique)
        {
            await _cliniqueService.ModifierCliniqueAsync(id, clinique);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerClinique(Guid id)
        {
            var result = await _cliniqueService.SupprimerCliniqueAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clinique>> ObtenirClinique(Guid id)
        {
            var clinique = await _cliniqueService.ObtenirCliniqueParIdAsync(id);
            return clinique == null ? NotFound() : Ok(clinique);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinique>>> ListerCliniques()
        {
            return Ok(await _cliniqueService.ListerCliniqueAsync());
        }

    }
}
