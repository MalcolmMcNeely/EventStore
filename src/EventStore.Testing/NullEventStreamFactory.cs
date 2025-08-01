using EventStore.Events.Streams;

namespace EventStore.Testing;

public class NullEventStreamFactory : IEventStreamFactory
{
    public IEventStream For(string streamName)
    {
        return new NullEventStream();
    }
}