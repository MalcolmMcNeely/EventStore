using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Transport;

public class InMemoryEventTransport : IEventTransport
{
    readonly Queue<IEvent> _eventQueue = new();

    public Task SendEventAsync(IEvent @event, CancellationToken token = default)
    {
        _eventQueue.Enqueue(@event);

        return Task.CompletedTask;
    }

    public Task<IEvent?> GetEventAsync(CancellationToken token = default)
    {
        if(!_eventQueue.TryDequeue(out var @event))
        {
            return Task.FromResult<IEvent?>(null);
        }

        return Task.FromResult(@event)!;
    }
}