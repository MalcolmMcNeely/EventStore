using EventStore.Events;
using EventStore.Events.Streams;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStream : IEventStream
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