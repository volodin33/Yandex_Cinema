using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace events.Kafka;

public record ProduceResultInfo(string Status, int Partition, long Offset);

public class KafkaProducer(IOptions<KafkaOptions> opt)
{
    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = opt.Value.Host,
            ClientId = $"{opt.Value.ClientId}-producer",
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 200,
            LingerMs = 5
        }
    ).Build();
    
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = null };

    public async Task<ProduceResultInfo> ProduceAsync(string topic, string key, object message, CancellationToken ct)
    {
        DeliveryResult<string, string> result;
        var value = JsonSerializer.Serialize(message, _jsonOptions);
        try
        {
            result = await _producer.ProduceAsync(
                topic,
                new Message<string, string>
                {
                    Key = key,
                    Value =value
                }, ct
            );
        }
        catch (ProduceException<string, string> ex)
        {
            return new ProduceResultInfo("failed", 0, 0);
        }

        return new ProduceResultInfo(result.Status.ToString(), result.Partition.Value, result.Offset.Value);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}