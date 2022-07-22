namespace SimpleKafka.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    void Subscribe<TEvent, THandler>(
        Dictionary<string, string>? config = null,
        string? topic = null,
        string? groupId = null,
        bool? enableAutoCommit = true);
}