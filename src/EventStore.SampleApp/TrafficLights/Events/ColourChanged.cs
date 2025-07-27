using EventStore.Events;

namespace EventStore.SampleApp.TrafficLights.Events;

public class ColourChanged : IEvent
{
    public Colour Colour { get; set; }
}