using EventStore.Events;

namespace EventStore.SampleApp.Azure.TrafficLights.Events;

public class ColourChanged : IEvent
{
    public Colour Colour { get; set; }
}