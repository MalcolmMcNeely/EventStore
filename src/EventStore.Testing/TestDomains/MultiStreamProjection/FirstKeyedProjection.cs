using EventStore.Projections;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class FirstKeyedProjection : IProjection
{
    public string? Id { get; set; }
    public int RowVersion { get; set; }
    public string? Data { get; set; }
}