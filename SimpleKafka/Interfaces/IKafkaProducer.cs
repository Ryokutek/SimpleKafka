using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace SimpleKafka.Interfaces;

public interface IKafkaProducer<TKey> : IDisposable
{
    Task<DeliveryResult<TKey?, string>> ProduceAsync<TEvent>(TEvent eventData, string topic, TKey? key = default);
}