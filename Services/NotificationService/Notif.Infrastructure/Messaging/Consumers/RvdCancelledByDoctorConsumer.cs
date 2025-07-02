using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class RvdCancelledByDoctorIntegrationEvent
    {
        public RendezVousDto RendezVous { get; set; }
        public string Raison { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class RvdCancelledByDoctorConsumer : KafkaConsumerBase<RvdCancelledByDoctorIntegrationEvent>
    {
        public RvdCancelledByDoctorConsumer(
            ConsumerConfig config,
            ILogger<RvdCancelledByDoctorConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("rdv-cancelled-by-doctor", config, logger, scopeFactory) { }

        protected override async Task HandleEventAsync(RvdCancelledByDoctorIntegrationEvent evt, IServiceProvider sp)
        {
            var notificationService = sp.GetRequiredService<INotificationApplicationService>();

            var notif = new CreateNotificationRequest(
                RecipientId: evt.RendezVous.PatientId,
                RecipientType: UserType.Patient,
                Type: NotificationType.AppointmentCancelledByDoctor,
                Title: "Rendez-vous annulé",
                Content: $"Votre rendez-vous prévu le {evt.RendezVous.DateHeure:dd/MM/yyyy à HH:mm} a été annulé. Raison : {evt.Raison}",
                Metadata: new Dictionary<string, object> {
                    { "RdvId", evt.RendezVous.Id },
                    { "Raison", evt.Raison }
                }
            );

            await notificationService.CreateNotificationAsync(notif);
        }
    }
}
