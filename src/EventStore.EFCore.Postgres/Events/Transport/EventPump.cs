using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventPump : IEventPump
{
    public Task PublishEventsAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}