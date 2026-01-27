using events.Models.API;
using Microsoft.Extensions.Options;

namespace events.Kafka.Consumers;

public class MovieEventsConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer<MovieEventApiRequest>> logger) : KafkaConsumer<MovieEventApiRequest>(options.Value.Topics.Movie, options, logger)
{
    private readonly ILogger<KafkaConsumer<MovieEventApiRequest>> _logger = logger;

    protected override void HandleMessage(MovieEventApiRequest message)
        =>_logger.LogInformation($"Movie event received. MovieId={message.MovieId}");
}