using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventStore;

public class EventForwarderBackgroundService(ILogger<EventForwarderBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Test");
            await Task.Delay(1000, stoppingToken);
        }
    }
}