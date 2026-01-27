namespace events.Models.API;

public record EventApiResponse
{
    public string Status { get; set; }
    public int Partition { get; set; }
    public long Offset { get; set; }
    public Event Event { get; set; }
}

public class Event
{
    public string Id { get; set; }
    public string Type { get; set; } 
    public DateTimeOffset Timestamp { get; set; }
    public object? Payload { get; set; }
}