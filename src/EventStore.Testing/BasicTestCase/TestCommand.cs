using EventStore.Commands;

namespace EventStore.Testing.BasicTestCase;

public class TestCommand : ICommand
{
    public required string Data { get; set; }
}