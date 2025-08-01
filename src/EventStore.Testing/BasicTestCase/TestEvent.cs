using EventStore.Events;

namespace EventStore.Testing.BasicTestCase;

public class TestEvent : IEvent
{
    public required string Data { get; set; }
}