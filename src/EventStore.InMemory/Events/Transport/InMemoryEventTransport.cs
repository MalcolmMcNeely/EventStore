using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Events.Transport;

public class InMemoryEventTransport : IEventTransport
{
    public Queue<IEvent> TransportQueue { get; } = new();

    public Task WriteEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        TransportQueue.Enqueue(@event);

        return Task.CompletedTask;
    }
}