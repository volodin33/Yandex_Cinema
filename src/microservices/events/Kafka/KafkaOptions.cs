namespace events.Kafka;

public class KafkaOptions
{
    public string Host { get; set; }
    public string ClientId { get; set; }
    public string GroupId { get; set; }
    public EventsTopics Topics { get; set; }
}

public class EventsTopics
{
    public string Movie { get; set; }
    public string User { get; set; }
    public string Payment { get; set; }
}