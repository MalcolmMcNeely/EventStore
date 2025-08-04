using EventStore.Commands.AggregateRoots;
using EventStore.EFCore.Postgres.Database;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.AggregateRoots;

public class AggregateRootRepository<TAggregateRoot>(IServiceScopeFactory serviceScopeFactory, IEventTransport transport, IEventStreamFactory eventStreamFactory) : IAggregateRootRepository<TAggregateRoot>  where TAggregateRoot : AggregateRoot, new()
{
    public async Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        return await dbContext.FindAsync<TAggregateRoot>([key], token) ?? new TAggregateRoot { Id = key };
    }

    public async Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        await dbContext.UpsertAsync(aggregateRoot, key);

        return true;
    }

    public async Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        var eventStream = eventStreamFactory.For(key);
        await eventStream.PublishAsync(@event, token);

        await transport.WriteEventAsync(@event, token);
    }
}