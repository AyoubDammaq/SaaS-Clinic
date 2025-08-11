using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;

namespace Notif.Infrastructure.Messaging.Consumers
{
    public class FactureAddedEvent
    {
        public FactureDto Facture { get; set; } = null!;
        public DateTime OccurredOn { get; set; }
    }

    public class FactureDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ConsultationId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime DateEmission { get; set; }
        public decimal MontantTotal { get; set; }
        public int Status { get; set; }
    }


    public class FactureAddedConsumer : KafkaConsumerBase<FactureAddedEvent>
    {
        private readonly ILogger<FactureAddedConsumer> _logger;
        public FactureAddedConsumer(
            ConsumerConfig config, 
            ILogger<FactureAddedConsumer> logger,
            INotificationApplicationService notificationService,
            IServiceScopeFactory scopeFactory)
            : base("facture-created", config, logger, scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task HandleEventAsync(FactureAddedEvent evt, IServiceProvider serviceProvider)
        {
            var facture = evt.Facture;

            if (facture == null || facture.PatientId == Guid.Empty)
            {
                _logger.LogWarning("Facture ou PatientId invalide dans l'événement.");
                return;
            }

            try
            {
                var notificationService = serviceProvider.GetRequiredService<INotificationApplicationService>();

                var command = new CreateNotificationRequest(
                    RecipientId: facture.PatientId,
                    RecipientType: UserType.Patient,
                    Type: NotificationType.FactureAdded,
                    Title: "Nouvelle facture disponible",
                    Content: $"Une nouvelle facture de {facture.MontantTotal:C} est disponible dans votre espace patient."
                );

                await notificationService.CreateNotificationAsync(command);
                _logger.LogInformation("Notification créée avec succès pour la facture {FactureId}", facture.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la notification pour la facture {FactureId}", facture.Id);
            }
        }
    }
}
