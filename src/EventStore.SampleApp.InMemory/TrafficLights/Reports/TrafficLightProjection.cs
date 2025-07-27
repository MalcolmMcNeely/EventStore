using EventStore.Projections;

namespace EventStore.SampleApp.InMemory.TrafficLights.Reports;

public class TrafficLightProjection : IProjection
{
    public string Id { get; set; }
    public Colour Colour { get; set; }
}