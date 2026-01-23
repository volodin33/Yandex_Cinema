namespace events.Models.API;

public class PaymentEventApiRequest : IEventRequest
{
    public int PaymentId { get; init; }
    public int UserId { get; init; }
    public float Amount { get; init; }
    public string Status { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }
    public string? MethodType { get; init; }
}