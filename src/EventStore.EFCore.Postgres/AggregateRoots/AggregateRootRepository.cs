using EventStore.Commands.AggregateRoots;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Events.Transport;

namespace EventStore.EFCore.Postgres.AggregateRoots;

public class AggregateRootRepository<TAggregateRoot>(EventStoreDbContext dbContext, IEventTransport transport, IEventStreamFactory eventStreamFactory) : IAggregateRootRepository<TAggregateRoot>  where TAggregateRoot : AggregateRoot, new()
{
    public async Task<TAggregateRoot> LoadAsync(string key, CancellationToken token = default)
    {
        return await dbContext.FindAsync<TAggregateRoot>([key], token) ?? new TAggregateRoot { Id = key };
    }

    public async Task<bool> SaveAsync(TAggregateRoot aggregateRoot, string key, CancellationToken token = default)
    {
        dbContext.Update(aggregateRoot);
        var result = await dbContext.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task SendEventAsync<TEvent>(TEvent @event, string key, CancellationToken token = default) where TEvent : class, IEvent
    {
        var eventStream = eventStreamFactory.For(key);
        await eventStream.PublishAsync(@event, token);

        await transport.WriteEventAsync(@event, token);
    }
}