using EventStore.Commands;

namespace EventStore.SampleApp.Domain.TrafficLights.Commands;

public class ChangeColour : ICommand
{
    public Colour Colour { get; set; }
}