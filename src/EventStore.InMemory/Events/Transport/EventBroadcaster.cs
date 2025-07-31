using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.InMemory.Events.Transport;

public class EventBroadcaster(IEventPump eventPump, EventDispatcher eventDispatcher) : IEventBroadcaster
{
    readonly EventPump? _inMemoryEventPump = eventPump as EventPump;

    public async Task BroadcastEventAsync(CancellationToken token)
    {
        if (_inMemoryEventPump is null)
        {
            return;
        }

        if (_inMemoryEventPump.EventPumpQueue.TryDequeue(out var @event))
        {
            await eventDispatcher.SendEventAsync(@event, token);
        }
    }
}