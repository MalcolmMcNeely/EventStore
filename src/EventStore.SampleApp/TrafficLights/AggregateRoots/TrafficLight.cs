using EventStore.Commands.AggregateRoots;
using EventStore.SampleApp.TrafficLights.Commands;
using EventStore.SampleApp.TrafficLights.Events;

namespace EventStore.SampleApp.TrafficLights.AggregateRoots;

public class TrafficLight : AggregateRoot
{
    Colour CurrentColour { get; set; }

    public TrafficLight()
    {
        Handles<ColourChanged>(OnColourChanged);
    }

    void OnColourChanged(ColourChanged @event)
    {
        CurrentColour = @event.Colour;
    }

    public Task ChangeColourAsync(ChangeColour command)
    {
        Update(new ColourChanged {  Colour = command.Colour });

        return Task.CompletedTask;
    }
}