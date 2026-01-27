using events.Models.API;
using Microsoft.Extensions.Options;

namespace events.Kafka.Consumers;

public class PaymentEventsConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer<PaymentEventApiRequest>> logger) : KafkaConsumer<PaymentEventApiRequest>(options.Value.Topics.Payment, options, logger)
{
    private readonly ILogger<KafkaConsumer<PaymentEventApiRequest>> _logger = logger;

    protected override void HandleMessage(PaymentEventApiRequest message) 
        => _logger.LogInformation($"Payment event received. PaymentId={message.PaymentId}");
}