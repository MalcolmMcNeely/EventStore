namespace EventStore.SampleApp.Domain.Accounts.Events;

public class AccountOpened : Event
{
    public required AccountModel AccountModel { get; set; }
}