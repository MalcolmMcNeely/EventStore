using EventStore.Events;

namespace EventStore.Testing.SimpleTestDomain;

public class TestEvent : IEvent
{
    public string CausationId { get; set; }
    public required string Data { get; set; }
}