using System.Text.Json.Serialization;

namespace events.Models.API;

public class PaymentEventApiRequest : IEventRequest
{
    [JsonPropertyName("payment_id")]
    public int PaymentId { get; init; }
    
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }
    
    public float Amount { get; init; }
    public string Status { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }
    
    [JsonPropertyName("method_type")]
    public string? MethodType { get; init; }
}