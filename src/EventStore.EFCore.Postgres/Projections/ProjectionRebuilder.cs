using EventStore.Projections;

namespace EventStore.EFCore.Postgres.Projections;

public class ProjectionRebuilder
{
    public async Task<bool> CanRebuildAsync(string key, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<T> RebuildAsync<T>(string key, CancellationToken token) where T : IProjection, new()
    {
        throw new NotImplementedException();
    }
}