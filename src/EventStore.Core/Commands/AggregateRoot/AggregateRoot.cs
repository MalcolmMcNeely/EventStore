using EventStore.Events;

namespace EventStore.Commands.AggregateRoot;

public class AggregateRoot
{
    public List<IEvent> NewEvents { get; } = new();
    
    protected void Update(IEvent @event)
    {
        NewEvents.Add(@event);
    }
}