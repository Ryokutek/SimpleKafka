using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using SimpleKafka.Interfaces;
using SimpleKafka.Models;

namespace SimpleKafka.Services;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly ILogger<IKafkaConsumer>? _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly Dictionary<string, string> _config;
    private IConsumer<Ignore, string>? _consumer;
    
    public event EventHandler<ReceivedEventArgs>? Received;

    public KafkaConsumer(Dictionary<string, string>? config, ILogger<IKafkaConsumer>? logger = null)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _config = config ?? new Dictionary<string, string>();
    }

    public void Consume(string topic, string groupId, bool enableAutoCommit)
    {
        _config.Add("group.id", groupId);
        _config.Add("enable.auto.commit", enableAutoCommit.ToString());
        _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        _consumer.Subscribe(topic);
        
        Task.Factory.StartNew(() =>
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<Ignore, string> consumeResult = _consumer.Consume(300);
                    
                    if (consumeResult is null) 
                        continue;
                    
                    var receivedEventArgs = new ReceivedEventArgs(
                        consumeResult.Topic, consumeResult.Message.Value, consumeResult);
                    
                    OnReceived(receivedEventArgs);
                }
                catch (Exception ex)
                {
                    _logger?.LogError("Consume error {Error}, Topic {Topic}", ex.Message, topic);
                }
            }
        }, _cancellationToken);
    }
    
    protected virtual void OnReceived(ReceivedEventArgs e)
    {
        Received?.Invoke(this, e);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _consumer?.Dispose();
        GC.SuppressFinalize(this);
    }
}