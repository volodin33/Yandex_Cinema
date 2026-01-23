using events.Models.API;
using Microsoft.Extensions.Options;

namespace events.Kafka.Consumers;

public class PaymentEventsConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer<PaymentEventApiRequest>> logger) : KafkaConsumer<PaymentEventApiRequest>(options.Value.Topics.Payment, options, logger)
{
    protected override void HandleMessage(PaymentEventApiRequest message) 
        => logger.LogInformation($"Payment event received. PaymentId={message.PaymentId}");
}