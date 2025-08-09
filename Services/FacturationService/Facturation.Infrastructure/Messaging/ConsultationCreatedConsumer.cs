using Confluent.Kafka;
using Facturation.Application.DTOs;
using Facturation.Application.FactureService.Commands.AddFacture;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Facturation.Infrastructure.Messaging
{
    public class ConsultationCreatedEvent
    {
        public ConsultationDto Consultation { get; set; }
        public DateTime OccurredOn { get; set; }
    }

    public class ConsultationDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedecinId { get; set; }
        public Guid ClinicId { get; set; }
        public int Type { get; set; }
        public DateTime DateConsultation { get; set; }
        public string Diagnostic { get; set; }
        public string Notes { get; set; }
    }

    public class ConsultationCreatedConsumer : IHostedService
    {
        private readonly ILogger<ConsultationCreatedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConsumer<Ignore, string> _consumer;

        public ConsultationCreatedConsumer(ILogger<ConsultationCreatedConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _scopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:9092",
                GroupId = "facturation-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => Consume(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        private async void Consume(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("consultation-created");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);
                    _logger.LogInformation($"Message reçu : {cr.Value}");

                    var message = JsonSerializer.Deserialize<ConsultationCreatedEvent>(cr.Value, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (message?.Consultation == null)
                    {
                        _logger.LogWarning("Le message reçu ne contient pas de consultation valide.");
                        continue;
                    }

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var tarifRepo = scope.ServiceProvider.GetRequiredService<ITarificationConsultationRepository>();

                        var consultation = message.Consultation;

                        var montant = await tarifRepo.GetMontantAsync(consultation.ClinicId, consultation.Type);

                        if (montant == null)
                        {
                            _logger.LogWarning("Aucun tarif trouvé pour la clinique {ClinicId} et le type {Type}", consultation.ClinicId, consultation.Type);
                            montant = 0;
                        }

                        var request = new CreateFactureRequest
                        {
                            PatientId = message.Consultation.PatientId,
                            ConsultationId = message.Consultation.Id,
                            ClinicId = message.Consultation.ClinicId,
                            MontantTotal = montant.Value 
                        };

                        var command = new AddFactureCommand(request);
                        await mediator.Send(command, cancellationToken);

                        _logger.LogInformation("Facture créée avec succès pour la consultation {ConsultationId}", message.Consultation.Id);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Erreur de consommation Kafka : {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du traitement du message Kafka.");
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }
    }

}
