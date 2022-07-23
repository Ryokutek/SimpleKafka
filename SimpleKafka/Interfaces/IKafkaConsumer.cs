using SimpleKafka.Models;

namespace SimpleKafka.Interfaces;

public interface IKafkaConsumer : IDisposable
{
    event EventHandler<ReceivedEventArgs> Received;
    public Task Consume(string topic, string groupId, bool enableAutoCommit);
}