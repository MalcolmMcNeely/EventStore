using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Transport;

public class InMemoryEventTransport : IEventTransport
{
    readonly Queue<IEvent> _eventQueue = new();

    public Task PublishEventAsync<T>(T @event, CancellationToken token = default) where T :  class, IEvent
    {
        _eventQueue.Enqueue(@event);

        return Task.CompletedTask;
    }

    public Task<IEvent?> ReceiveEventAsync(CancellationToken token = default)
    {
        if(!_eventQueue.TryDequeue(out var @event))
        {
            return Task.FromResult<IEvent?>(null);
        }

        return Task.FromResult(@event)!;
    }
}