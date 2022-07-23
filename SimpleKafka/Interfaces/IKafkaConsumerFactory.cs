using Confluent.Kafka;

namespace SimpleKafka.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    void Subscribe<TEvent, THandler>(
        ConsumerConfig? config = null,
        string? topic = null,
        string? groupId = null,
        bool? enableAutoCommit = true);
}