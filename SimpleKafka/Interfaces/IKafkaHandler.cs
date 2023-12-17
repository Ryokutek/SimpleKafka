using System.Threading.Tasks;

namespace SimpleKafka.Interfaces;

public interface IKafkaHandler<in TEvent>
{
    Task HandleAsync(TEvent @event);
}