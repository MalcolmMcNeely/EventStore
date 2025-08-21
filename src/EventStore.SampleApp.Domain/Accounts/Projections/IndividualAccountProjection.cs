namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class IndividualAccountProjection : Projection
{
    public string Name { get; set; }
    public bool IsClosed { get; set; }
    public decimal Balance { get; set; }
}