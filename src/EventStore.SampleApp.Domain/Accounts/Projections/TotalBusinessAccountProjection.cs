namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class TotalBusinessAccountProjection : Projection
{
    public List<string> Accounts { get; set; } = new();
    public decimal Balance { get; set; }
}