using EventStore.Azure.Events.Streams;
using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.Azure.Events.Transport;

public class AzureEventTransport(EventStreamFactory eventStreamFactory) : IEventTransport
{
    readonly EventStream _allStream = eventStreamFactory.For(Defaults.Streams.AllStreamPartition);

    public async Task WriteEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        await _allStream.PublishAsync(@event, token);
    }
}