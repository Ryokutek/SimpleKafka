using Confluent.Kafka;
using SimpleKafka.Interfaces;
using SimpleKafka.Models;

namespace SimpleKafka.Services;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly Dictionary<string, string> _config;
    private IConsumer<Ignore, string> _consumer = null!;
    
    public event EventHandler<ReceivedEventArgs> Received = null!;

    public KafkaConsumer(Dictionary<string, string>? config)
    {
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
                    
                    var deliverEventArgs = new ReceivedEventArgs(
                        consumeResult.Topic, consumeResult.Message.Value, consumeResult);
                    
                    OnReceived(deliverEventArgs);
                }
                catch (Exception ex)
                {
                    //TODO
                }
            }
        }, _cancellationToken);
    }
    
    protected virtual void OnReceived(ReceivedEventArgs e)
    {
        Received.Invoke(this, e);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _consumer.Dispose();
        GC.SuppressFinalize(this);
    }
}