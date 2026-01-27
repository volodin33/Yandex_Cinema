using events.Kafka;
using events.Models.API;
using events.Models.Comands;
using MediatR;
using Microsoft.Extensions.Options;

namespace events.Handlers;

public class ProduceEventRequestHandler(KafkaProducer producer, IOptions<KafkaOptions> options) : IRequestHandler<ProduceEventRequest, ProduceResultInfo>
{
    private readonly string _movieTopic = options.Value.Topics.Movie;
    private readonly string _userTopic = options.Value.Topics.User;
    private readonly string _paymentTopic = options.Value.Topics.Payment;
    
    public Task<ProduceResultInfo> Handle(ProduceEventRequest request, CancellationToken cancellationToken)
    {
        return request.Request switch
        {
            MovieEventApiRequest movie => producer.ProduceAsync(_movieTopic, $"{movie.MovieId}", movie, cancellationToken),
            UserEventApiRequest user => producer.ProduceAsync(_userTopic, $"{user.UserId}", user, cancellationToken),
            PaymentEventApiRequest payment => producer.ProduceAsync(_paymentTopic, $"{payment.PaymentId}", payment, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported message type: {request.Request.GetType().FullName}")
        };
    }
}