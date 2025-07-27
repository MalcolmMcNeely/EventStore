namespace EventStore.Events;

public interface IEventDispatcher
{
    public void SendEvent<T>(T @event, CancellationToken token = default) where T : IEvent;
}