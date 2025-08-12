using EventStore.Commands.Dispatching;
using EventStore.SampleApp.Domain;
using EventStore.SampleApp.Domain.TrafficLights.Commands;

namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public class ChangeColourBackgroundService(ICommandDispatcher commandDispatcher) : BackgroundService
{
    Colour _currentColour = Colour.Green;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await commandDispatcher.DispatchAsync(new ChangeColour { Colour = _currentColour }, token);

            _currentColour = NextColour();

            await Task.Delay(2000, token);
        }
    }

    Colour NextColour()
    {
        switch (_currentColour)
        {
            case Colour.Red:
                return Colour.Green;
            case Colour.Yellow:
                return Colour.Red;
            case Colour.Green:
                return Colour.Yellow;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}