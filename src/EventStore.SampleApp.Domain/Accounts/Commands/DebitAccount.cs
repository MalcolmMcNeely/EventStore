namespace EventStore.SampleApp.Domain.Accounts.Commands;

public class DebitAccount :  Command
{
    public required string AccountName { get; set; }
    public decimal Amount { get; set; }
}