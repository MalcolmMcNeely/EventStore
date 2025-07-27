using EventStore.Events;

namespace EventStore.Transport;

public interface IEventTransport
{
    public Task SendEventAsync(IEvent @event, CancellationToken token = default);
}