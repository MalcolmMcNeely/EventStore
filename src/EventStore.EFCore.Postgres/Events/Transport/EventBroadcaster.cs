using System.Text.Json;
using EventStore.EFCore.Postgres.Database;
using EventStore.Events;
using EventStore.Events.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.Events.Transport;

public class EventBroadcaster(IServiceScopeFactory scopeFactory, EventDispatcher eventDispatcher) : IEventBroadcaster
{
    public async Task BroadcastEventAsync(CancellationToken token = default)
    {
        using var scope = scopeFactory.CreateScope();

        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        await using var transaction = await scopedDbContext.Database.BeginTransactionAsync(token).ConfigureAwait(false);

        var queuedEvents = await scopedDbContext.QueuedEvents.ToListAsync(cancellationToken: token).ConfigureAwait(false);

        if (queuedEvents.Count == 0)
        {
            return;
        }

        foreach (var queuedEvent in queuedEvents)
        {
            var envelope = queuedEvent.Envelope;
            var @event = (IEvent)JsonSerializer.Deserialize(envelope.Body, Type.GetType(envelope.Type)!)!;
            
            await eventDispatcher.SendEventAsync(@event, token).ConfigureAwait(false);
        }

        scopedDbContext.QueuedEvents.RemoveRange(queuedEvents);

        await scopedDbContext.SaveChangesAsync(token).ConfigureAwait(false);
        await transaction.CommitAsync(token).ConfigureAwait(false);
    }
}