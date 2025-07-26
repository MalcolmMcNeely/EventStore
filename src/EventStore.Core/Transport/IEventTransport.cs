using EventStore.Events;

namespace EventStore.Transport;

public interface IEventTransport
{
    public void SendEvent(IEvent @event);
}