using EventStore.Events;
using EventStore.Events.Streams;

namespace EventStore.Testing;

public class NullEventStream : IEventStream
{
    public Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IEvent> GetAllEventsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}