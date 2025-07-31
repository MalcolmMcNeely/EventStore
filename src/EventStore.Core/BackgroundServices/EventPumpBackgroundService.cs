using EventStore.Events.Transport;
using Microsoft.Extensions.Hosting;

namespace EventStore.BackgroundServices;

public class EventPumpBackgroundService(IEventPump eventPump): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await eventPump.PublishEventsAsync(token);

            await Task.Delay(100, token);
        }
    }
}