using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class RvdConfirmedIntegrationEvent
    {
        public RendezVousConfirmedEvent RendezVous { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class RendezVousConfirmedEvent
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedecinId { get; set; }
        public DateTime DateHeure { get; set; }
        public int Statut { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateCreation { get; set; }
    }


    public class RvdConfirmedConsumer : KafkaConsumerBase<RvdConfirmedIntegrationEvent>
    {
        public RvdConfirmedConsumer(
            ConsumerConfig config,
            ILogger<RvdConfirmedConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("rdv-confirmed", config, logger, scopeFactory)
        {
        }

        protected override async Task HandleEventAsync(RvdConfirmedIntegrationEvent evt, IServiceProvider serviceProvider)
        {
            var _notificationService = serviceProvider.GetRequiredService<INotificationApplicationService>();

            var rdv = evt.RendezVous;

            // Optionnel : appeler un service pour obtenir le nom du patient et la spécialité du médecin
            var patientNom = "Nom Patient"; // À remplacer par appel réel
            var specialty = "Généraliste"; // À remplacer par appel réel

            var patientNotification = new CreateNotificationRequest(
                RecipientId: rdv.PatientId,
                RecipientType: UserType.Patient,
                Type: NotificationType.AppointmentConfirmation,
                Title: "Rendez-vous confirmé",
                Priority: NotificationPriority.High,
                Content: $"Votre rendez-vous est confirmé pour le {rdv.DateHeure:dd/MM/yyyy à HH:mm}.",
                Metadata: new Dictionary<string, object>
                {
                { "RdvId", rdv.Id },
                { "Specialty", specialty }
                }
            );

            await _notificationService.CreateNotificationAsync(patientNotification);
        }
    }
}
