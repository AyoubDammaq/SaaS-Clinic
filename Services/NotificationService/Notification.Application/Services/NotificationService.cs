using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Interfaces;

namespace Notification.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Domain.Entities.Notification> CréerNotificationAsync(Domain.Entities.Notification notification)
        {
            notification.Id = Guid.NewGuid();
            if (notification.DateEnvoi == default)
                notification.DateEnvoi = DateTime.UtcNow;
            notification.Statut = StatutNotification.NON_LU;

            return await _repository.AddAsync(notification);
        }

        public async Task<IEnumerable<Domain.Entities.Notification>> RécupérerNotificationsUtilisateurAsync(
            Guid utilisateurId, StatutNotification? statut = null, TypeNotification? type = null)
        {
            return await _repository.GetByUserAsync(utilisateurId, statut, type);
        }

        public async Task<Domain.Entities.Notification?> RécupérerParIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> MarquerCommeLueAsync(Guid id)
        {
            return await _repository.MarkAsReadAsync(id);
        }

        public async Task MarquerToutesCommeLuesAsync(Guid utilisateurId)
        {
            await _repository.MarkAllAsReadAsync(utilisateurId);
        }

        public async Task<bool> SupprimerNotificationAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<PreferenceNotification?> ObtenirPréférencesAsync(Guid utilisateurId)
        {
            return await _repository.GetPreferencesAsync(utilisateurId);
        }

        public async Task DéfinirPréférencesAsync(PreferenceNotification preferences)
        {
            await _repository.SetPreferencesAsync(preferences);
        }

        public async Task<bool> RenvoyerNotificationAsync(Guid id)
        {
            var notification = await _repository.GetByIdAsync(id);
            if (notification == null)
                return false;

            // TODO: Implémenter la logique d'envoi réel (email, SMS, in-app)
            notification.DateEnvoi = DateTime.UtcNow;
            notification.Statut = StatutNotification.NON_LU;
            await _repository.AddAsync(notification);
            return true;
        }
    }
}
