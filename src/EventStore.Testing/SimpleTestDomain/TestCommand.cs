using EventStore.Commands;

namespace EventStore.Testing.SimpleTestDomain;

public class TestCommand : ICommand
{
    public required string Data { get; set; }
}