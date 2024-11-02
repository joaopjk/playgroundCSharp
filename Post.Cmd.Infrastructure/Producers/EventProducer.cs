using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ?? "localhost:9092",
    };

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

        if (deliveryResult.Status != PersistenceStatus.NotPersisted)
            throw new Exception($"Could not produce {@event.GetType().Name} to topic {topic} due to following reason: {deliveryResult.Message}");
        
    }
}