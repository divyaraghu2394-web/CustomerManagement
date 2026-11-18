using Confluent.Kafka;
using System.Text.Json;
using Messaging.Contracts;

namespace CustomerManagement.API.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(
        string topic,
        string key,
        CustomerEvent customerEvent)
    {
        string jsonMessage = JsonSerializer.Serialize(customerEvent);

        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = jsonMessage
        };

        var result = await _producer.ProduceAsync(topic, kafkaMessage);

        Console.WriteLine(
            $"JSON message delivered to {result.TopicPartitionOffset}");
    }
}