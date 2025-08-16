using System.Threading.Channels;
using EventStore.Commands.Dispatching;
using EventStore.SampleApp.Domain;
using EventStore.SampleApp.Domain.TrafficLights.Commands;

namespace EventStore.Blazor.EFCore.Postgres.BackgroundServices;

public class ChangeColourBackgroundService(IServiceScopeFactory scopeFactory, ICommandDispatcher commandDispatcher) : BackgroundService
{
    Colour _currentColour = Colour.Green;
    bool _isRunning = false;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var channel  = scopedServices.GetRequiredService<Channel<ChangeColourBackgroundServiceRequest>>();

            if (channel.Reader.TryRead(out var request))
            {
                if (request.Toggle is true)
                {
                    _isRunning = !_isRunning;
                }
                else
                {
                    _isRunning = request.Running switch
                    {
                        true => true,
                        false => false,
                        null => _isRunning
                    };
                }
            }

            if (_isRunning)
            {
                await commandDispatcher.DispatchAsync(new ChangeColour { Colour = _currentColour }, token);

                _currentColour = NextColour();
            }

            await Task.Delay(3000, token);
        }
    }

    Colour NextColour()
    {
        return _currentColour switch
        {
            Colour.Red => Colour.Green,
            Colour.Yellow => Colour.Red,
            Colour.Green => Colour.Yellow,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}