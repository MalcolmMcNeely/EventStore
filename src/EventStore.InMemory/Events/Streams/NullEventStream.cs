using EventStore.Events;
using EventStore.Events.Streams;

namespace EventStore.InMemory.Events.Streams;

public class NullEventStream : IEventStream
{
    public Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IEvent> GetAllEventsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IEvent> GetEventsSinceAsync(int fromIndex, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}