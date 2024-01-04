﻿using Confluent.Kafka;

namespace SimpleKafka.Interfaces;

public interface IKafkaProducer<TKey> : IDisposable
{
    Task<DeliveryResult<TKey?, string>> ProduceAsync<TEvent>(TEvent eventData, string? topic = null, TKey? key = default);
}