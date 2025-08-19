using EventStore.Commands;

namespace EventStore.Testing.TestDomains.SimpleTestDomain;

public class TestCommand : ICommand
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public required string Data { get; set; }
}