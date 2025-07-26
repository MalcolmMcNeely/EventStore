using EventStore.Transport;

namespace EventStore.Events;

public interface IEventDispatcher
{
    public void SendEvent(IEvent @event);
}

public class EventDispatcher(IEventTransport transport) : IEventDispatcher
{
    public void SendEvent(IEvent @event)
    {
        transport.SendEvent(@event);
    }
}
