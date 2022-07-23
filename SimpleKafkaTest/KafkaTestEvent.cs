using SimpleKafka.Models;

namespace SimpleKafkaTest;

public class KafkaTestEvent : BaseEvent
{
    public string Message { get; set; }
}