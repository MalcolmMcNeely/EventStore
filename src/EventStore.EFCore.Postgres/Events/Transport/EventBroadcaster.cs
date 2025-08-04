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

        await using var transaction = await scopedDbContext.Database.BeginTransactionAsync(token);

        var queuedEvents = await scopedDbContext.QueuedEvents.Take(10).ToListAsync(cancellationToken: token);

        if (queuedEvents.Count == 0)
        {
            return;
        }

        foreach (var @event in queuedEvents)
        {
            await eventDispatcher.SendEventAsync(@event.Content, token);
        }

        scopedDbContext.QueuedEvents.RemoveRange(queuedEvents);

        await scopedDbContext.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}