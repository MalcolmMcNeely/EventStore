using EventStore.EFCore.Postgres.Commands;
using EventStore.EFCore.Postgres.Database;
using Microsoft.EntityFrameworkCore;

namespace EventStore.Blazor.EFCore.Postgres.Services.Commands;

public class CommandService(IServiceScopeFactory serviceScopeFactory) : ICommandService
{
    public async Task<(bool HasNewCommands, List<CommandEntity> Commands)> GetCommandsSince(DateTime time, CancellationToken token)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        var commands = await scopedDbContext.Commands
            .Where(x => x.TimeStamp > time)
            .OrderBy(x => x.TimeStamp)
            .ToListAsync(cancellationToken: token)
            .ConfigureAwait(false);

        return (commands.Count != 0, commands);
    }
}