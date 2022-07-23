using SimpleKafka.Interfaces;

namespace SimpleKafkaTest;

public class KafkaTestHandler : IEventHandler<KafkaTestEvent>
{
    public Task Handle(KafkaTestEvent @event)
    {
        return Task.CompletedTask;
    }
}