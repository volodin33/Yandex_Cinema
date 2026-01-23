using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace events.Kafka.Consumers;

public abstract class KafkaConsumer<T>(string topic, IOptions<KafkaOptions> options, ILogger<KafkaConsumer<T>> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaOptions = options.Value;
        var cfg = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Host,
            GroupId = kafkaOptions.GroupId,
            ClientId = $"{kafkaOptions.ClientId}-consumer-{topic}",
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        var jsonOptions = new JsonSerializerOptions {PropertyNamingPolicy = null};

        using var consumer = new ConsumerBuilder<string, string>(cfg)
            .SetErrorHandler((_, e) => logger.LogError($"Kafka error: {e.Reason}"))
            .Build();

        consumer.Subscribe(topic);

        logger.LogInformation($"Consumer started. Topic={topic}, GroupId={kafkaOptions.GroupId}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var msg = JsonSerializer.Deserialize<T>(result.Message.Value, jsonOptions);
                HandleMessage(msg);
                consumer.Commit(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ConsumeException. Topic={topic}. {ex.Message}");
            }
        }

        consumer.Close();
        return Task.CompletedTask;
    }

    protected abstract void HandleMessage(T message);
}