using EventStore.Projections;

namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class Projection : IProjection
{
    public int RowVersion { get; set; }
    public string Id { get; set; }
}