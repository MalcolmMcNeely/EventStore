using EventStore.EFCore.Postgres.Commands;

namespace EventStore.Blazor.EFCore.Postgres.Services.Commands;

public interface ICommandService
{
    public Task<List<CommandEntity>> GetCommandsSince(int index, CancellationToken token = default);
}