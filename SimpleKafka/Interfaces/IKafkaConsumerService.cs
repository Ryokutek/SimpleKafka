using System;
using SimpleKafka.Modules;

namespace SimpleKafka.Interfaces;

public interface IKafkaConsumerService : IDisposable
{
    event EventHandler<ReceivedEventDetails> Received;
    public void Consume(string topic, string groupId, bool enableAutoCommit);
}