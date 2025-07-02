using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using PatientManagementService.Domain.Interfaces.Messaging;
using System.Text.Json;

namespace PatientManagementService.Infrastructure.Messaging
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(IConfiguration config)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = jsonMessage
            };

            await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
        }
    }
}
