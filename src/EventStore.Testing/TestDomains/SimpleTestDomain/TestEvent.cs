using EventStore.Events;

namespace EventStore.Testing.TestDomains.SimpleTestDomain;

public class TestEvent : IEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CausationId { get; set; } = Guid.NewGuid().ToString();
    public string Data { get; set; } = Guid.NewGuid().ToString();
}