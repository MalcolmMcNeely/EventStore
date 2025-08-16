using EventStore.EFCore.Postgres.Commands;

namespace EventStore.Blazor.EFCore.Postgres.Services.Commands;

public interface ICommandService
{
    Task<(bool HasNewCommands, List<CommandEntity> Commands)> GetCommandsSince(DateTime time, CancellationToken token = default);
}