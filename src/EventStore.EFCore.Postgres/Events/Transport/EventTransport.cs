using EventStore.Events;
using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventTransport : IEventTransport
{
    public Task WriteEventAsync<T>(T @event, CancellationToken token = default) where T : class, IEvent
    {
        throw new NotImplementedException();
    }
}