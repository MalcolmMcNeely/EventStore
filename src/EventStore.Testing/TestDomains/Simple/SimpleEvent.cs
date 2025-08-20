using EventStore.Events;

namespace EventStore.Testing.TestDomains.Simple;

public class SimpleEvent : IEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public string Data { get; set; } = Guid.NewGuid().ToString();
}