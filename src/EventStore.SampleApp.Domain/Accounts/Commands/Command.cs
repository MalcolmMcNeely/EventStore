using EventStore.Commands;

namespace EventStore.SampleApp.Domain.Accounts.Commands;

public class Command : ICommand
{
    public required string User { get; set; }
    public string CausationId { get; set; }
}