using EventStore.Projections;

namespace EventStore.SampleApp.Domain.TrafficLights.Projections;

public class TrafficLightProjection : IProjection
{
    public string Id { get; set; }
    public byte[] RowVersion { get; set; }
    public Colour Colour { get; set; }
}