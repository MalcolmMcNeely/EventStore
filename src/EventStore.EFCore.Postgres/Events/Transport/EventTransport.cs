using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventTransport(IEventStreamFactory eventStreamFactory) : IEventTransport
{
    public async Task WriteEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        await eventStreamFactory.For(Defaults.Streams.AllStreamPartition).PublishAsync(@event, token).ConfigureAwait(false);
    }
}