using System.Text.Json;
using EventStore.Commands;
using EventStore.Commands.Dispatching;
using EventStore.EFCore.Postgres.Database;
using EventStore.EFCore.Postgres.Events;
using EventStore.EFCore.Postgres.Events.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.Commands;

public class CommandAudit(IServiceScopeFactory serviceScopeFactory) : ICommandAudit
{
    readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task PublishAsync<T>(T command, CancellationToken token) where T : ICommand
    {
        await _semaphore.WaitAsync(token).ConfigureAwait(false);

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
            var lastRowKey = await dbContext.Commands.MaxAsync(x => (int?)x.RowKey, token).ConfigureAwait(false) ?? 0;

            var eventEntity = new CommandEntity
            {
                Key = Defaults.Commands.CommandPartition,
                RowKey = lastRowKey + 1,
                TimeStamp = DateTime.UtcNow,
                CommandType = command.GetType().Name,
                CausationId = command.CausationId,
                Content = JsonSerializer.Serialize((object)command)
            };

            await dbContext.Commands.AddAsync(eventEntity, token).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(token).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}