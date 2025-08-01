using EventStore.Events.Streams;

namespace EventStore.InMemory.Events.Streams;

public class NullEventStreamFactory : IEventStreamFactory
{
    public IEventStream For(string streamName)
    {
        return new NullEventStream();
    }
}