using Confluent.Kafka;
using Facturation.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Facturation.Infrastructure.Messaging
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration config, ILogger<KafkaProducer> logger)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var jsonMessage = JsonSerializer.Serialize(message, options);
            _logger.LogInformation("Message JSON envoyé : {Json}", jsonMessage);
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = jsonMessage
            };

            await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
        }
    }
}
