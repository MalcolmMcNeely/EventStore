namespace EventStore.Events.Transport;

public interface IEventTransport
{
    public Task WriteEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent;
}