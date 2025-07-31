using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Events.Transport;

public class InMemoryEventPump(IEventTransport eventTransport) : IEventPump
{
    public Queue<IEvent> EventPumpQueue { get; } = new();

    readonly InMemoryEventTransport? _inMemoryEventTransport = eventTransport as InMemoryEventTransport;

    public Task PublishEventsAsync(CancellationToken token = default)
    {
        if (_inMemoryEventTransport is null)
        {
            return Task.CompletedTask;
        }

        if (_inMemoryEventTransport.TransportQueue.TryDequeue(out var @event))
        {
            EventPumpQueue.Enqueue(@event);
        }

        return Task.CompletedTask;
    }
}