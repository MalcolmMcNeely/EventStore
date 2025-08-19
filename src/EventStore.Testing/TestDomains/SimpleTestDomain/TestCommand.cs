using EventStore.Commands;

namespace EventStore.Testing.TestDomains.SimpleTestDomain;

public class TestCommand : ICommand
{
    public required string CausationId { get; set; }
    public required string Data { get; set; }
}