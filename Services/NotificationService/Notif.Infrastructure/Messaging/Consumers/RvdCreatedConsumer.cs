using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class RvdCreatedIntegrationEvent
    {
        public RendezVousDto RendezVous { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class RendezVousDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedecinId { get; set; }
        public DateTime DateHeure { get; set; }
        public int Statut { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateCreation { get; set; }
    }

    public class RvdCreatedConsumer : KafkaConsumerBase<RvdCreatedIntegrationEvent>
    {
        public RvdCreatedConsumer(
            ConsumerConfig config,
            ILogger<RvdCreatedConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("rdv-created", config, logger, scopeFactory) { }

        protected override async Task HandleEventAsync(RvdCreatedIntegrationEvent evt, IServiceProvider sp)
        {
            var notificationService = sp.GetRequiredService<INotificationApplicationService>();

            var notification = new CreateNotificationRequest(
                RecipientId: evt.RendezVous.PatientId,
                RecipientType: UserType.Patient,
                Type: NotificationType.AppointmentCreatetd,
                Title: "Nouveau rendez-vous créé",
                Priority: NotificationPriority.Low,
                Content: $"Votre rendez-vous a été créé pour le {evt.RendezVous.DateHeure:dd/MM/yyyy à HH:mm}.",
                Metadata: new Dictionary<string, object>
                {
                    { "RdvId", evt.RendezVous.Id }
                }
            );

            await notificationService.CreateNotificationAsync(notification);
        }
    }
}
