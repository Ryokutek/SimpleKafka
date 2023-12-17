using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using SimpleKafka.Modules;

namespace SimpleKafka.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    Task SubscribeAsync<TEvent, THandler>(ConsumerConfig? config = null,
        string? topicPrefix = null,
        string? groupId = null,
        bool? enableAutoCommit = true)
        where THandler : class, IKafkaHandler<TEvent>
        where TEvent : BaseEvent;
}