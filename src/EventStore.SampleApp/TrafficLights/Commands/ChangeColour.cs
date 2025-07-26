using EventStore.Commands;

namespace EventStore.SampleApp.TrafficLights.Commands;

public class ChangeColour : ICommand
{
    public string Colour { get; set; }
}