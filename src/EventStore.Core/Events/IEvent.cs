namespace EventStore.Events;

public interface IEvent
{
    public string CausationId { get; set; }
}