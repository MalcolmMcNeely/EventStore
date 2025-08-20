using EventStore.Commands;

namespace EventStore.Testing.TestDomains.MultiStreamProjection;

public class MultiStreamProjectionCommand : ICommand
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public required string Stream { get; set; }
    public required string Data { get; set; }
}