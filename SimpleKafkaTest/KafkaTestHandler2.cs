using SimpleKafka.Interfaces;

namespace SimpleKafkaTest;

public class KafkaTestHandler2 : IEventHandler<KafkaTestEvent>
{
    public Task Handle(KafkaTestEvent @event)
    {
        return Task.CompletedTask;
    }
}