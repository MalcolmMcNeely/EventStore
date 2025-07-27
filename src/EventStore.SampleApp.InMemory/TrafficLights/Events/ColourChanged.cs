using EventStore.Events;

namespace EventStore.SampleApp.InMemory.TrafficLights.Events;

public class ColourChanged : IEvent
{
    public Colour Colour { get; set; }
}