using EventStore.Events;

namespace EventStore.Testing.TestDomains.Idempotency;

public class IdempotencyEvent : IEvent
{
    public string CausationId { get; set; }
    public string Stream { get; set; }
    public string Data { get; set; }
}