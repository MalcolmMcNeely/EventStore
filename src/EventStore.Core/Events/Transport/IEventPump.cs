namespace EventStore.Events.Transport;

public interface IEventPump
{
    public Task PublishEventsAsync(CancellationToken token = default);
}