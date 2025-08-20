using EventStore.Commands;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleCommand : ICommand
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public required string Data { get; set; }
}