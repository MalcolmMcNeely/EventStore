namespace EventStore.Events;

public interface IEventDispatcher
{
    public Task SendEventAsync<T>(T @event, CancellationToken token = default) where T : IEvent;
}