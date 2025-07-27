using EventStore.Projections;
using EventStore.SampleApp.InMemory.TrafficLights.Projections;
using Microsoft.Extensions.Hosting;

namespace EventStore.SampleApp.InMemory;

public class PrintColourBackgroundService(IProjectionRepository<TrafficLightProjection> repository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var projection = await repository.LoadAsync(nameof(TrafficLightProjection), token);

            if (projection is not null)
            {
                Console.WriteLine(projection!.Colour);
            }

            await Task.Delay(1000, token);
        }
    }
}