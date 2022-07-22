using Confluent.Kafka;

namespace SimpleKafka.Interfaces;

public interface IKafkaProducer<TKey> : IDisposable
{
    Task<DeliveryResult<TKey?, string>> PublishAsync<TEvent>(TEvent eventData, string? topic = null, TKey? key = default);
}