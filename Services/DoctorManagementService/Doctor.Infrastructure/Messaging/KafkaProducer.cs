using Confluent.Kafka;
using Doctor.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Doctor.Infrastructure.Messaging
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

            try
            {
                var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
                Console.WriteLine($"✅ Message envoyé sur topic '{topic}', offset {result.Offset}");
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"❌ Kafka error: {ex.Error.Reason}");
            }
        }
    }
}
