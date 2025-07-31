namespace EventStore.Events.Streams;

public interface IEventStream
{
    Task PublishAsync(IEvent entity, CancellationToken token = default);
}