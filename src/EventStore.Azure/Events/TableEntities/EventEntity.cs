using EventStore.Azure.Azure;

namespace EventStore.Azure.Events.TableEntities;

public class EventEntity : TableEntity
{
    public required string EventType { get; set; }
    public bool IsLarge { get; set; }
    public required string CausationId { get; set; }
    public required string Content { get; set; }
}