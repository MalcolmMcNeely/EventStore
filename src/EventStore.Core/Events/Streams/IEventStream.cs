namespace EventStore.Events.Streams;

public interface IEventStream
{
    Task PublishAsync(IEvent entity, CancellationToken token = default);
    Task<bool> ExistsAsync(CancellationToken token = default);
    IAsyncEnumerable<IEvent> GetAllEventsAsync(CancellationToken token = default);
}