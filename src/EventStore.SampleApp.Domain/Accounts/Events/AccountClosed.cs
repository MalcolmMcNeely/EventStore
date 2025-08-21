namespace EventStore.SampleApp.Domain.Accounts.Events;

public class AccountClosed : Event
{
    public required string AccountName { get; set; }
}