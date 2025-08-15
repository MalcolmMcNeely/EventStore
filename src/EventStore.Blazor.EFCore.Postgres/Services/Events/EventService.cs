using EventStore.EFCore.Postgres.Database;
using EventStore.EFCore.Postgres.Events;
using Microsoft.EntityFrameworkCore;

namespace EventStore.Blazor.EFCore.Postgres.Services.Events;

public class EventService(IServiceScopeFactory serviceScopeFactory) : IEventService
{
    public async Task<List<EventStreamEntity>> GetEventsSince(int index, CancellationToken token)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        return await scopedDbContext.EventStreams.Skip(index).ToListAsync(cancellationToken: token).ConfigureAwait(false);
    }
}