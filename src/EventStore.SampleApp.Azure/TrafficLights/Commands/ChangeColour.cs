using EventStore.Commands;

namespace EventStore.SampleApp.Azure.TrafficLights.Commands;

public class ChangeColour : ICommand
{
    public Colour Colour { get; set; }
}