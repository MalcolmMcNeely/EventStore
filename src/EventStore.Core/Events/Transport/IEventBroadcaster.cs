namespace EventStore.Events.Transport;

public interface IEventBroadcaster
{
    public Task BroadcastEventAsync(CancellationToken token);
}