namespace EventStore.SampleApp.Domain.Accounts.Commands;

public class OpenAccount : Command
{
    public required string AccountName { get; set; }
    public AccountType Type { get; set; }
}