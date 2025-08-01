using EventStore.Projections;
using EventStore.SampleApp.Domain.TrafficLights.Projections;
using Microsoft.Extensions.Hosting;

namespace EventStore.SampleApp.Domain;

public class PrintColourBackgroundService(IProjectionRepository<TrafficLightProjection> repository) : BackgroundService
{
    Colour _currentColour;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(1000, token);

            var projection = await repository.LoadAsync(nameof(TrafficLightProjection), token);

            if (projection is null)
            {
                continue;
            }

            if (projection.Colour == _currentColour)
            {
                continue;
            }

            _currentColour = projection.Colour;

            Console.WriteLine(_currentColour);
        }
    }
}