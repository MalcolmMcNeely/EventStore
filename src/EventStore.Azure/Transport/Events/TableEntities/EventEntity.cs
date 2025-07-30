namespace EventStore.Azure.Transport.Events.TableEntities;

public class EventEntity : TableEntity
{
    public required string EventType { get; set; }
    public bool IsLarge { get; set; }
    public required string Content { get; set; }
}