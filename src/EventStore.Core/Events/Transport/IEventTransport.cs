namespace EventStore.Events.Transport;

public interface IEventTransport
{
    public Task SendEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent;
    public Task<IEvent?> GetEventAsync(CancellationToken token = default);
}