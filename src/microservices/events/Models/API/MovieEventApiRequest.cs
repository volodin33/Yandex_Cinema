namespace events.Models.API;

public class MovieEventApiRequest : IEventRequest
{
    public int MovieId { get; init; }
    public string Title { get; init; }
    public string Action { get; init; }
    public int? UserId { get; init; }
    public float? Rating { get; init; }
    public string[]? Genres { get; init; }
    public string? Description { get; init; }
}