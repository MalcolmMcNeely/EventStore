using EventStore.EFCore.Postgres.Commands;
using EventStore.EFCore.Postgres.Database;
using Microsoft.EntityFrameworkCore;

namespace EventStore.Blazor.EFCore.Postgres.Services.Commands;

public class CommandService(IServiceScopeFactory serviceScopeFactory) : ICommandService
{
    public async Task<List<CommandEntity>> GetCommandsSince(int index, CancellationToken token)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        return await scopedDbContext.Commands
            .OrderBy(x => x.Key)
            .ThenBy(x => x.RowKey)
            .Skip(index)
            .ToListAsync(cancellationToken: token)
            .ConfigureAwait(false);
    }
}