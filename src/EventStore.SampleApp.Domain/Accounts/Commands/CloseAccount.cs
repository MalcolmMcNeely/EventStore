namespace EventStore.SampleApp.Domain.Accounts.Commands;

public class CloseAccount : Command
{
    public required string AccountName { get; set; }
}