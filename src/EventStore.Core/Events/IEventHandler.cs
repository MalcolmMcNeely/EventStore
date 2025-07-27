namespace EventStore.Events;

public interface IEventHandler<T> where T : IEvent
{
    public Task HandleAsync(T @event, CancellationToken token = default);
}