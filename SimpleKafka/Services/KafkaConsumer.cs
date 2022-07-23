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
    private readonly ConsumerConfig _config;
    private IConsumer<Ignore, string>? _consumer;
    
    public event EventHandler<ReceivedEventArgs>? Received;

    public KafkaConsumer(ConsumerConfig? config, ILogger<IKafkaConsumer>? logger = null)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _config = config ?? new ConsumerConfig();
    }

    public async Task Consume(string topic, string groupId, bool enableAutoCommit)
    {
        _config.GroupId = groupId;
        _config.EnableAutoCommit = enableAutoCommit;
        _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        await SubscribeWithRetry(topic);
        
        await Task.Factory.StartNew(() =>
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
                catch (Exception e)
                {
                    _logger?.LogError("Consume error {Error}. Topic {Topic}", e.Message, topic);
                }
            }
        }, _cancellationToken);
    }

    private async Task SubscribeWithRetry(string topic)
    {
        try
        {
            _consumer?.Subscribe(topic);
        }
        catch (Exception e)
        {
            _logger?.LogError("Consumer error {Error}. Topic {Topic}", e.Message, topic);
            await Task.Delay(5000, _cancellationToken);
        }
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