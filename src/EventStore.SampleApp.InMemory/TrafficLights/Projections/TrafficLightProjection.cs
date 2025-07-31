using EventStore.Projections;

namespace EventStore.SampleApp.InMemory.TrafficLights.Projections;

public class TrafficLightProjection : IProjection
{
    public string? Id { get; set; }
    public Colour Colour { get; set; }
}