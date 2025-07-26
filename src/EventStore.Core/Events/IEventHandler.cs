namespace EventStore.Events;

public interface IEventHandler<T> where T : IEvent
{
    public void Handle(T @event);
}