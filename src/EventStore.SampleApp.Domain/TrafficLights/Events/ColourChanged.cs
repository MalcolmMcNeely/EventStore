using EventStore.Events;

namespace EventStore.SampleApp.Domain.TrafficLights.Events;

public class ColourChanged : IEvent
{
    public Colour Colour { get; set; }
}