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
        await semaphore.WaitAsync(token);

        try
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                var eventEntity = new EventStreamEntity
                {
                    Key = streamName,
                    TimeStamp = DateTime.UtcNow,
                    EventType = entity.GetType().Name,
                    Envelope = Envelope.Create(entity)
                };

                await dbContext.EventStreams.AddAsync(eventEntity, token);
                await dbContext.SaveChangesAsync(token);
            }
        }
        catch (Exception ex)
        {
            int i = 5;
            throw;
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
        return await dbContext.EventStreams.Where(x => x.Key == streamName).AnyAsync(token);
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
}