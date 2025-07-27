using EventStore.Transport;

namespace EventStore.Events;

public interface IEventDispatcher
{
    public void SendEvent(IEvent @event, CancellationToken token = default);
}

public class EventDispatcher(IEventTransport transport) : IEventDispatcher
{
    public void SendEvent(IEvent @event, CancellationToken token = default)
    {
        transport.SendEventAsync(@event, token);
    }
}
