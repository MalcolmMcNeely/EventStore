using EventStore.Events.Transport;

namespace EventStore.Events;

public class EventDispatcher(IEventTransport transport) : IEventDispatcher
{
    public void SendEvent(IEvent @event, CancellationToken token = default)
    {
        transport.SendEventAsync(@event, token);
    }
}
