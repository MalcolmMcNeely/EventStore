using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventBroadcaster : IEventBroadcaster
{
    public Task BroadcastEventAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}