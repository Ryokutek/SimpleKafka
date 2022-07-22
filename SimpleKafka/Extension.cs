using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleKafka.Interfaces;
using SimpleKafka.Services;

namespace SimpleKafka;

public static class Extension
{
    public static void AddKafkaProducer<TKey>(this IServiceCollection serviceCollection, Dictionary<string, string> config)
    {
        serviceCollection.AddSingleton<IKafkaProducer<TKey>>(provider =>
        {
            ILogger<IKafkaProducer<TKey>>? logger = provider.GetService<ILogger<IKafkaProducer<TKey>>>();
            return new KafkaProducer<TKey>(logger, config);
        });
    }
    
    public static void AddKafkaConsumersFactory<TKey>(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
    }
}