using EventStore.Events;
using EventStore.Events.Streams;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStreamFactory(EventStoreDbContext dbContext, Lazy<IEventTypeRegistration> eventTypeRegistration) : IEventStreamFactory
{
    public IEventStream For(string streamName)
    {
        return new EventStream(dbContext, streamName, eventTypeRegistration);
    }
}