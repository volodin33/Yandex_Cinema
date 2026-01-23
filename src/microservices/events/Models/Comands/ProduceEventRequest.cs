using events.Kafka;
using events.Models.API;
using MediatR;

namespace events.Models.Comands;

public record ProduceEventRequest(IEventRequest Request) : IRequest<ProduceResultInfo>;