using events.Models.API;
using Microsoft.Extensions.Options;

namespace events.Kafka.Consumers;

public class UserEventsConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer<UserEventApiRequest>> logger) : KafkaConsumer<UserEventApiRequest>(options.Value.Topics.User, options, logger)
{
    private readonly ILogger<KafkaConsumer<UserEventApiRequest>> _logger = logger;

    protected override void HandleMessage(UserEventApiRequest message) 
        => _logger.LogInformation($"User event received. UserId={message.UserId}");
}