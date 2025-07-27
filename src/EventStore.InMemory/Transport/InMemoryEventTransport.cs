using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Transport;

public class InMemoryEventTransport : IEventTransport
{
    Queue<IEvent> _eventQueue = new();

    public Task SendEventAsync(IEvent @event, CancellationToken token = default)
    {
        _eventQueue.Enqueue(@event);

        return Task.CompletedTask;
    }

    public Task<IEvent> GetEventAsync(CancellationToken token = default)
    {
        return Task.FromResult(_eventQueue.Dequeue());
    }
}