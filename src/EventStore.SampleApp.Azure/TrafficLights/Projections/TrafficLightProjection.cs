using EventStore.Projections;

namespace EventStore.SampleApp.Azure.TrafficLights.Projections;

public class TrafficLightProjection : IProjection
{
    public string? Id { get; set; }
    public Colour Colour { get; set; }
}