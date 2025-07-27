namespace EventStore.Events.Transport;

public interface IEventTransport
{
    public Task SendEventAsync(IEvent @event, CancellationToken token = default);
}