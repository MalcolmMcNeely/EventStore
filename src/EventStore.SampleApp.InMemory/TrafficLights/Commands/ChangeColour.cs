using EventStore.Commands;

namespace EventStore.SampleApp.InMemory.TrafficLights.Commands;

public class ChangeColour : ICommand
{
    public Colour Colour { get; set; }
}