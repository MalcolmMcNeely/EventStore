using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Events.Transport;

public class EventPump(IEventTransport eventTransport) : IEventPump
{
    public Queue<IEvent> EventPumpQueue { get; } = new();

    readonly EventTransport? _inMemoryEventTransport = eventTransport as EventTransport;

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