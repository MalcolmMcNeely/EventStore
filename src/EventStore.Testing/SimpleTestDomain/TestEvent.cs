using EventStore.Events;

namespace EventStore.Testing.SimpleTestDomain;

public class TestEvent : IEvent
{
    public required string Data { get; set; }
}