using EventStore.AggregateRoots;
using EventStore.EFCore.Postgres.Database;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.AggregateRoots;

public sealed class AggregateRootRepository<TAggregateRoot>(IServiceScopeFactory serviceScopeFactory, IEventTransport transport, IEventStreamFactory eventStreamFactory) : IAggregateRootRepository<TAggregateRoot>  where TAggregateRoot : AggregateRoot, new()
{
    public async Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        using var semaphorePool = await DbSemaphoreSlimPool.AcquireAsync(token);
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        return await dbContext.Set<TAggregateRoot>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(key), token)
            .ConfigureAwait(false) ?? new TAggregateRoot { Id = key };
    }

    public async Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        using var semaphorePool = await DbSemaphoreSlimPool.AcquireAsync(token);
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        await dbContext.UpsertAsync(aggregateRoot, [key], token).ConfigureAwait(false);

        return true;
    }

    public async Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        var eventStream = eventStreamFactory.For(key);
        await eventStream.PublishAsync(@event, token).ConfigureAwait(false);

        await transport.WriteEventAsync(@event, token).ConfigureAwait(false);
    }
}