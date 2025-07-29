using EventStore.Events;
using EventStore.Events.Transport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventStore.BackgroundServices;

internal sealed class EventForwarderBackgroundService(IEventTransport eventTransport, IEventDispatcher eventDispatcher, ILogger<EventForwarderBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var @event = await eventTransport.GetEventAsync(token);

            if (@event is not null)
            {
                await eventDispatcher.SendEventAsync(@event, token);
            }

            await Task.Delay(100, token);
        }
    }
}