using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class RvdCancelledIntegrationEvent
    {
        public RendezVousCancelledEvent RendezVous { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class RendezVousCancelledEvent
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedecinId { get; set; }
        public DateTime DateHeure { get; set; }
        public int Statut { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateCreation { get; set; }
    }

    public class RvdCancelledConsumer : KafkaConsumerBase<RvdCancelledIntegrationEvent>
    {
        public RvdCancelledConsumer(
            ConsumerConfig config,
            ILogger<RvdCancelledConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("rdv-cancelled", config, logger, scopeFactory)
        {
        }

        protected override async Task HandleEventAsync(RvdCancelledIntegrationEvent evt, IServiceProvider serviceProvider)
        {
            var _notificationService = serviceProvider.GetRequiredService<INotificationApplicationService>();

            var rdv = evt.RendezVous;

            var command = new CreateNotificationRequest(
                RecipientId: rdv.PatientId,
                RecipientType: UserType.Patient,
                Type: NotificationType.AppointmentCancellation,
                Title: "Rendez-vous annulé",
                Priority: NotificationPriority.High,
                Content: $"Votre rendez-vous prévu le {rdv.DateHeure:dd/MM/yyyy HH:mm} a été annulé."
            );

            await _notificationService.CreateNotificationAsync(command);
        }
    }
}
