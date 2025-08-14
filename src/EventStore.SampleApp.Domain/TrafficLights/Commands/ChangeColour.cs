using EventStore.Commands;

namespace EventStore.SampleApp.Domain.TrafficLights.Commands;

public class ChangeColour : ICommand
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public Colour Colour { get; set; }
}