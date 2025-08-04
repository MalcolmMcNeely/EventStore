using EventStore.Projections;
using EventStore.SampleApp.Domain.TrafficLights.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.SampleApp.Domain;

public class PrintColourBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    Colour _currentColour;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(1000, token);

            var repository = serviceProvider.GetService<IProjectionRepository<TrafficLightProjection>>();

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