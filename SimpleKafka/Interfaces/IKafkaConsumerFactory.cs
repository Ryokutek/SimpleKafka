using Confluent.Kafka;

namespace SimpleKafka.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    Task Subscribe<TEvent, THandler>(ConsumerConfig? config = null,
        string? topic = null,
        string? groupId = null,
        bool? enableAutoCommit = true);
}