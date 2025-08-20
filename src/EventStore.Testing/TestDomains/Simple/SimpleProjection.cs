using EventStore.Projections;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleProjection : IProjection
{
    public string? Id { get; set; }
    public int RowVersion { get; set; }
    public string? Data { get; set; }
}