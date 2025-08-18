namespace EventStore.SampleApp.Domain.Accounts.Events;

public class AccountClosed : Event
{
    public required AccountModel AccountModel { get; set; }
}