using EventStore.Projections;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyProjection : IProjection
{
    public int RowVersion { get; set; }
    public string Id { get; set; }
    public string Data { get; set; }
}