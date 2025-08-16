using EventStore.EFCore.Postgres.Database;
using EventStore.EFCore.Postgres.Events;
using Microsoft.EntityFrameworkCore;

namespace EventStore.Blazor.EFCore.Postgres.Services.Events;

public class EventService(IServiceScopeFactory serviceScopeFactory) : IEventService
{
    public async Task<(bool HasNewEvents, List<EventStreamEntity> Events)> GetEventsSince(DateTime time, CancellationToken token)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        var events = await scopedDbContext.EventStreams
            .Where(x => x.TimeStamp > time)
            .OrderBy(x => x.TimeStamp)
            .ToListAsync(cancellationToken: token)
            .ConfigureAwait(false);

        return (events.Count != 0,  events);
    }
}