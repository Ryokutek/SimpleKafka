using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleKafka.Interfaces;

public interface IKafkaTopicFactory
{
    Task CreateTopicIfNotExistAsync(string topic, Dictionary<string, string> config);
}