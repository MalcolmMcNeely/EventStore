using EventStore.Events;

namespace EventStore.SampleApp.Domain.TrafficLights.Events;

public class ColourChanged : IEvent
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public Colour Colour { get; set; }
}