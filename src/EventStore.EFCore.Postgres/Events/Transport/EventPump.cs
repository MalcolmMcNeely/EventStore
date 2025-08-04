using System.Runtime.CompilerServices;
using EventStore.EFCore.Postgres.Database;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.Events.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventPump(IServiceScopeFactory scopeFactory) : IEventPump
{
    public async Task PublishEventsAsync(CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();

        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        var scopedCursorFactory = scope.ServiceProvider.GetRequiredService<EventCursorFactory>();
        var cursor = await scopedCursorFactory.GetOrAddCursorAsync(Defaults.Cursors.AllStreamCursor, token);
        var newEvents = ReceiveEventsAsync(scopedDbContext, cursor, token);
        var eventCount = 0;
        
        await foreach (var @event in newEvents)
        {
            scopedDbContext.QueuedEvents.Add(new QueuedEventEntity
            {
                TimeStamp = DateTime.UtcNow,
                EventType = @event.EventType,
                Envelope = @event.Envelope,
            });
            eventCount++;
        }

        cursor.LastSeenEvent += eventCount;

        await scopedDbContext.SaveChangesAsync(token);
        await scopedCursorFactory.SaveCursorAsync(cursor, token);
    }

    async IAsyncEnumerable<EventStreamEntity> ReceiveEventsAsync(EventStoreDbContext scopedDbContext, EventCursorEntity eventCursor, [EnumeratorCancellation] CancellationToken token = default)
    {
        var entities = scopedDbContext.EventStreams
            .Where(x => x.Key == Defaults.Streams.AllStreamPartition && x.RowKey > eventCursor.LastSeenEvent)
            .OrderByDescending(x => x.RowKey)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var entity in entities.WithCancellation(token))
        {
            yield return entity;
        }
    }
}