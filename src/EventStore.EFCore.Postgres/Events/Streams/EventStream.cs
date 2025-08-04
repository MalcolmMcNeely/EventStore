using System.Runtime.CompilerServices;
using EventStore.Events;
using EventStore.Events.Streams;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStream(EventStoreDbContext dbContext, string streamName, Lazy<IEventTypeRegistration> eventTypeRegistration) : IEventStream
{
    public async Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        var eventEntity = new EventStreamEntity
        {
            Key = streamName,
            TimeStamp = DateTime.UtcNow.TimeOfDay,
            EventType = entity.GetType().Name,
            Content = entity
        };

        await dbContext.EventStreams.AddAsync(eventEntity, token);
        await dbContext.SaveChangesAsync(token);
    }

    public async Task<bool> ExistsAsync(CancellationToken token = default)
    {
        return await dbContext.EventStreams.Where(x => x.Key == streamName).AnyAsync(token);
    }

    public async IAsyncEnumerable<IEvent> GetAllEventsAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        var events = dbContext.EventStreams
            .Where(x => x.Key == streamName)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var e in events.WithCancellation(token))
        {
            yield return e.Content;
        }
    }
}