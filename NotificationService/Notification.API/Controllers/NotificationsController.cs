using Microsoft.AspNetCore.Mvc;
using Notification.Application.DTOs;
using Notification.Application.Interfaces;
using Notification.Domain.Enums;
using Notification.Domain.Entities;
using Notification.Application.Services;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        // 1. Créer une notification
        [HttpPost]
        public async Task<IActionResult> Créer([FromBody] Domain.Entities.Notification notification)
        {
            var result = await _service.CréerNotificationAsync(notification);
            return CreatedAtAction(nameof(RécupérerParId), new { id = result.Id }, result);
        }

        // 2. Récupérer toutes les notifications d’un utilisateur
        [HttpGet("utilisateur/{utilisateurId}")]
        public async Task<IActionResult> RécupérerParUtilisateur(Guid utilisateurId, [FromQuery] StatutNotification? statut, [FromQuery] TypeNotification? type)
        {
            var list = await _service.RécupérerNotificationsUtilisateurAsync(utilisateurId, statut, type);
            return Ok(list);
        }

        // 3. Récupérer une notification spécifique
        [HttpGet("{id}")]
        public async Task<IActionResult> RécupérerParId(Guid id)
        {
            var notif = await _service.RécupérerParIdAsync(id);
            if (notif == null) return NotFound();
            return Ok(notif);
        }

        // 4. Marquer une notification comme lue
        [HttpPatch("{id}/marquer-lue")]
        public async Task<IActionResult> MarquerCommeLue(Guid id)
        {
            var success = await _service.MarquerCommeLueAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // 5. Marquer toutes les notifications comme lues
        [HttpPatch("utilisateur/{utilisateurId}/marquer-toutes-lues")]
        public async Task<IActionResult> MarquerToutes(Guid utilisateurId)
        {
            await _service.MarquerToutesCommeLuesAsync(utilisateurId);
            return NoContent();
        }

        // 6. Supprimer une notification
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var success = await _service.SupprimerNotificationAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // 7a. Modifier les préférences utilisateur
        [HttpPut("preferences/{utilisateurId}")]
        public async Task<IActionResult> DéfinirPréférences(Guid utilisateurId, [FromBody] PreferenceNotification prefs)
        {
            prefs.UtilisateurId = utilisateurId;
            await _service.DéfinirPréférencesAsync(prefs);
            return NoContent();
        }

        // 7b. Obtenir les préférences
        [HttpGet("preferences/{utilisateurId}")]
        public async Task<IActionResult> ObtenirPréférences(Guid utilisateurId)
        {
            var prefs = await _service.ObtenirPréférencesAsync(utilisateurId);
            if (prefs == null) return NotFound();
            return Ok(prefs);
        }

        // 8. Renvoyer une notification échouée
        [HttpPost("{id}/resend")]
        public async Task<IActionResult> Renvoyer(Guid id)
        {
            var success = await _service.RenvoyerNotificationAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }

}
