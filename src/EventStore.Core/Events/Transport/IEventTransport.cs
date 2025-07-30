namespace EventStore.Events.Transport;

public interface IEventTransport
{
    public Task PublishEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent;
    public Task<IEvent?> ReceiveEventAsync(CancellationToken token = default);
}