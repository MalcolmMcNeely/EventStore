using EventStore.Events.Transport;
using Microsoft.Extensions.Hosting;

namespace EventStore.BackgroundServices;

internal sealed class EventBroadcasterBackgroundService(IEventBroadcaster eventBroadcaster) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await eventBroadcaster.BroadcastEventAsync(token);

            await Task.Delay(100, token);
        }
    }
}