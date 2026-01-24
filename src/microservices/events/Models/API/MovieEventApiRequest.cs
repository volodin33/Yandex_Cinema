using System.Text.Json.Serialization;

namespace events.Models.API;

public class MovieEventApiRequest : IEventRequest
{
    [JsonPropertyName("movie_id")]
    public int MovieId { get; init; }
    public string Title { get; init; }
    public string Action { get; init; }
    
    [JsonPropertyName("user_id")]
    public int? UserId { get; init; }
    
    public float? Rating { get; init; }
    public string[]? Genres { get; init; }
    public string? Description { get; init; }
}