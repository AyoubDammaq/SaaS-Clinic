using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Notif.Infrastructure.Messaging
{
    public abstract class KafkaConsumerBase<TEvent> : BackgroundService
    {
        private readonly string _topic;
        private readonly ConsumerConfig _config;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        protected KafkaConsumerBase(string topic, ConsumerConfig config, ILogger logger, IServiceScopeFactory scopeFactory)
        {
            _topic = topic;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // Permet aux classes filles d'utiliser des services Scoped
        protected abstract Task HandleEventAsync(TEvent message, IServiceProvider serviceProvider);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config).Build();

            // ✅ Attente jusqu’à ce que le topic existe (polling)
            using (var adminClient = new AdminClientBuilder(_config).Build())
            {
                bool topicAvailable = false;

                while (!stoppingToken.IsCancellationRequested && !topicAvailable)
                {
                    try
                    {
                        var metadata = adminClient.GetMetadata(_topic, TimeSpan.FromSeconds(5));
                        var topicMeta = metadata.Topics.FirstOrDefault(t => t.Topic == _topic);

                        if (topicMeta != null && !topicMeta.Error.IsError)
                        {
                            _logger.LogInformation("✅ Topic Kafka '{Topic}' est disponible.", _topic);
                            topicAvailable = true;
                        }
                        else
                        {
                            _logger.LogWarning("⏳ Topic '{Topic}' indisponible ou non encore créé. Nouvelle tentative dans 3s...", _topic);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur pendant la récupération des métadonnées Kafka pour le topic '{Topic}'", _topic);
                    }

                    if (!topicAvailable)
                        await Task.Delay(3000, stoppingToken).ConfigureAwait(false);
                }
            }

            consumer.Subscribe(_topic);
            _logger.LogInformation("🔔 Kafka abonné au topic : {Topic}", _topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);
                        if (string.IsNullOrWhiteSpace(result?.Message?.Value))
                        {
                            _logger.LogWarning("⚠️ Message Kafka vide reçu sur topic '{Topic}'", _topic);
                            continue;
                        }

                        var evt = JsonSerializer.Deserialize<TEvent>(result.Message.Value);
                        if (evt == null)
                        {
                            _logger.LogWarning("⚠️ Impossible de désérialiser le message reçu : {Value}", result.Message.Value);
                            continue;
                        }

                        _logger.LogInformation("📩 Message Kafka reçu sur '{Topic}' : {Message}", _topic, result.Message.Value);

                        using var scope = _scopeFactory.CreateScope();
                        await HandleEventAsync(evt, scope.ServiceProvider).ConfigureAwait(false);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "❌ ConsumeException sur le topic Kafka '{Topic}'", _topic);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "❌ Erreur de désérialisation du message Kafka sur le topic '{Topic}'", _topic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Erreur inconnue lors de la consommation sur le topic '{Topic}'", _topic);
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}

