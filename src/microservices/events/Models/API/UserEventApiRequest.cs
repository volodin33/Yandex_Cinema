using System.Text.Json.Serialization;

namespace events.Models.API;

public class UserEventApiRequest : IEventRequest
{
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }
    
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string Action { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }
}