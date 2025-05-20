using Notification.Application.DTOs;
using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Domain.Entities.Notification> CréerNotificationAsync(Domain.Entities.Notification notification);
        Task<IEnumerable<Domain.Entities.Notification>> RécupérerNotificationsUtilisateurAsync(Guid utilisateurId, StatutNotification? statut = null, TypeNotification? type = null);
        Task<Domain.Entities.Notification?> RécupérerParIdAsync(Guid id);
        Task<bool> MarquerCommeLueAsync(Guid id);
        Task MarquerToutesCommeLuesAsync(Guid utilisateurId);
        Task<bool> SupprimerNotificationAsync(Guid id);
        Task<PreferenceNotification?> ObtenirPréférencesAsync(Guid utilisateurId);
        Task DéfinirPréférencesAsync(PreferenceNotification preferences);
        Task<bool> RenvoyerNotificationAsync(Guid id);
    }
}
