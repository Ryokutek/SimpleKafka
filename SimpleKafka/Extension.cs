using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using SimpleKafka.Interfaces;
using SimpleKafka.Services;

namespace SimpleKafka;

public static class Extension
{
    public static void AddKafkaProducer<TKey>(this IServiceCollection serviceCollection, ProducerConfig config)
    {
        serviceCollection.AddSingleton<IKafkaProducer<TKey>>(_ => new KafkaProducerService<TKey>(config));
    }
    
    public static void AddKafkaConsumersFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IKafkaTopicFactory, KafkaTopicFactory>();
        serviceCollection.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
    }
}