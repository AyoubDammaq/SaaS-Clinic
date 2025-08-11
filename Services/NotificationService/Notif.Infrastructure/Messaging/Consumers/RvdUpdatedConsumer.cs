using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class RvdUpdatedIntegrationEvent
    {
        public RDVUpdatedEvent RendezVous { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class RDVUpdatedEvent
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedecinId { get; set; }
        public DateTime DateHeure { get; set; }
        public int Statut { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateCreation { get; set; }
    }


    public class RvdUpdatedConsumer : KafkaConsumerBase<RvdUpdatedIntegrationEvent>
    {
        public RvdUpdatedConsumer(
            ConsumerConfig config,
            ILogger<RvdUpdatedConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("rdv-updated", config, logger, scopeFactory)
        {
        }

        protected override async Task HandleEventAsync(RvdUpdatedIntegrationEvent evt, IServiceProvider serviceProvider)
        {
            var _notificationService = serviceProvider.GetRequiredService<INotificationApplicationService>();

            var rdv = evt.RendezVous;

            var notifyPatient = new CreateNotificationRequest(
                RecipientId: rdv.PatientId,
                RecipientType: UserType.Patient,
                Type: NotificationType.AppointmentUpdated,
                Title: "Rendez-vous modifié",
                Content: $"Votre rendez-vous a été mis à jour : nouvelle date {rdv.DateHeure:dd/MM/yyyy 'à' HH:mm}.",
                Metadata: new Dictionary<string, object>
                {
                { "RdvId", rdv.Id }
                }
            );

            var notifyMedecin = new CreateNotificationRequest(
                RecipientId: rdv.MedecinId,
                RecipientType: UserType.Doctor,
                Type: NotificationType.AppointmentUpdated,
                Title: "Modification de rendez-vous",
                Content: $"Le rendez-vous du patient prévu le {rdv.DateHeure:dd/MM/yyyy 'à' HH:mm} a été mis à jour.",
                Metadata: new Dictionary<string, object>
                {
                    { "RdvId", rdv.Id }
                }
            );

            await _notificationService.CreateNotificationAsync(notifyPatient);
            await _notificationService.CreateNotificationAsync(notifyMedecin); 
        }
    }

}
