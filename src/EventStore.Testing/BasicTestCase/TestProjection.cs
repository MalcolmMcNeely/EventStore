using EventStore.Projections;

namespace EventStore.Testing.BasicTestCase;

public class TestProjection : IProjection
{
    public string? Id { get; set; }
    public byte[] RowVersion { get; set; }
    public string? Data { get; set; }
}