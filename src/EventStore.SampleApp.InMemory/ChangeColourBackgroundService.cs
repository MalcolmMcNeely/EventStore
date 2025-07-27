using EventStore.Commands;
using EventStore.SampleApp.InMemory.TrafficLights.Commands;
using Microsoft.Extensions.Hosting;

namespace EventStore.SampleApp.InMemory;

public class ChangeColourBackgroundService(ICommandDispatcher commandDispatcher) : BackgroundService
{
    Colour _currentColour = Colour.Red;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _currentColour = NextColour();

            await commandDispatcher.DispatchAsync(new ChangeColour { Colour = _currentColour }, token);

            await Task.Delay(1000, token);
        }
    }

    Colour NextColour()
    {
        switch (_currentColour)
        {
            case Colour.Red:
                return Colour.Yellow;
            case Colour.Yellow:
                return Colour.Green;
            case Colour.Green:
                return Colour.Red;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}