using EventStore.Events.Streams;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStreamFactory : IEventStreamFactory
{
    public IEventStream For(string streamName)
    {
        throw new NotImplementedException();
    }
}