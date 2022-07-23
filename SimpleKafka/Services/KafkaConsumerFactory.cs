using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleKafka.Interfaces;
using SimpleKafka.Models;

namespace SimpleKafka.Services;

public class KafkaConsumerFactory : IKafkaConsumerFactory
{
    private readonly ILogger<IKafkaConsumerFactory>? _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, IKafkaConsumer> _consumers = new();
    private readonly Dictionary<string, List<EventHandlerTypes>> _eventHandlerModels = new();

    public KafkaConsumerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<ILogger<IKafkaConsumerFactory>>();
    } 
    
    public void Subscribe<TEvent, THandler>(
        Dictionary<string, string>? config = null,
        string? topic = null, 
        string? groupId = null, 
        bool? enableAutoCommit = true)
    {
        string eventName = typeof(TEvent).Name;
        if (string.IsNullOrEmpty(topic)) {
            topic = eventName;
        }
        
        string handlerName = typeof(THandler).Name;
        if (string.IsNullOrEmpty(groupId)) {
            groupId = handlerName;
        }

        List<EventHandlerTypes> eventHandlerModels = new List<EventHandlerTypes>();
        
        if (!_eventHandlerModels.ContainsKey(topic))
            _consumers[topic] = BuildConsumer(topic, groupId, enableAutoCommit, config);
        else
            eventHandlerModels = _eventHandlerModels[topic];
        
        eventHandlerModels.Add(new EventHandlerTypes(typeof(TEvent), typeof(THandler)));
        
        _eventHandlerModels[topic] = eventHandlerModels;
    }

    private IKafkaConsumer BuildConsumer(
        string topic,
        string groupId, 
        bool? enableAutoCommit,
        Dictionary<string, string>? config)
    {
        IKafkaConsumer kafkaConsumer = new KafkaConsumer(config, _serviceProvider.GetService<ILogger<IKafkaConsumer>>());
        kafkaConsumer.Consume(topic, groupId, enableAutoCommit ?? true);
        kafkaConsumer.Received += async (sender, args) => await KafkaConsumerOnReceived(sender, args);
        return kafkaConsumer;
    }

    private async Task KafkaConsumerOnReceived(object? _, ReceivedEventArgs e)
    {
        string topic = e.Topic;
        List<EventHandlerTypes> eventHandlerModels = _eventHandlerModels[topic];
        
        foreach (EventHandlerTypes eventHandlerModel in eventHandlerModels)
        {
            object? @event = JsonConvert.DeserializeObject(e.Message, eventHandlerModel.Event);
            if (@event is not BaseEvent baseEvent) {
                 continue;
            }
            
            object eventHandler = ActivatorUtilities.CreateInstance(_serviceProvider, eventHandlerModel.Handler);
            
            if (eventHandler is IEventHandler<BaseEvent> handler) {
                await handler.Handle(baseEvent);
            }
        }
    }
    
    public void Dispose() 
    {
        _consumers.ToList().ForEach(x => x.Value.Dispose());
        GC.SuppressFinalize(this);
    }
}