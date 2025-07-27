namespace EventStore.Events;

public interface IEventDispatcher
{
    public void SendEvent(IEvent @event, CancellationToken token = default);
}