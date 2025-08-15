using System.Runtime.CompilerServices;
using System.Text.Json;
using EventStore.EFCore.Postgres.Database;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.Events;
using EventStore.Events.Streams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStream(IServiceScopeFactory serviceScopeFactory, string streamName, SemaphoreSlim semaphore) : IEventStream
{
    public async Task PublishAsync(IEvent entity, CancellationToken token = default)
    {
        await semaphore.WaitAsync(token).ConfigureAwait(false);

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
            var lastRowKey = await dbContext.EventStreams
                .Where(x => x.Key == streamName)
                .MaxAsync(x => (int?)x.RowKey, token)
                .ConfigureAwait(false) ?? 0;

            var eventEntity = new EventStreamEntity
            {
                Key = streamName,
                RowKey = lastRowKey + 1,
                TimeStamp = DateTime.UtcNow,
                EventType = entity.GetType().Name,
                CausationId = entity.CausationId,
                Envelope = Envelope.Create(entity)
            };

            await dbContext.EventStreams.AddAsync(eventEntity, token).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(token).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<bool> ExistsAsync(CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        return await dbContext.EventStreams.Where(x => x.Key == streamName).AnyAsync(token).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<IEvent> GetAllEventsAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        var events = dbContext.EventStreams
            .Where(x => x.Key == streamName)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var entity in events.WithCancellation(token))
        {
            var @event = JsonSerializer.Deserialize(entity.Envelope.Body, Type.GetType(entity.Envelope.Type)!);

            yield return (IEvent)@event!;
        }
    }

    public async Task<int> GetCountAsync(CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        return await dbContext.EventStreams.Where(x => x.Key == streamName).CountAsync(token).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<IEvent> GetEventsSinceAsync(int fromIndex, CancellationToken token = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        var events = dbContext.EventStreams
            .Where(x => x.Key == streamName)
            .Skip(fromIndex)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var entity in events.WithCancellation(token))
        {
            var @event = JsonSerializer.Deserialize(entity.Envelope.Body, Type.GetType(entity.Envelope.Type)!);

            yield return (IEvent)@event!;
        }
    }
}