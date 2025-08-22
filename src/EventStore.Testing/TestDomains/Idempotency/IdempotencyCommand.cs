

using EventStore.Commands;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyCommand : ICommand
{
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public string Stream { get; set; }
    public string Data { get; set; }
}