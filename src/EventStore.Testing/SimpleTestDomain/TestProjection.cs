using EventStore.Projections;

namespace EventStore.Testing.SimpleTestDomain;

public class TestProjection : IProjection
{
    public string? Id { get; set; }
    public int RowVersion { get; set; }
    public string? Data { get; set; }
}