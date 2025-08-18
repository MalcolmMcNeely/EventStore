namespace EventStore.SampleApp.Domain.Accounts.Events;

public class AccountDebited : Event
{
    public required string AccountName { get; set; }
    public decimal Amount { get; set; }
}