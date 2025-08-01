using EventStore.Projections;

namespace EventStore.EFCore.Postgres.Projections;

public class ProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection
{
    public Task<T?> LoadAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(T projection, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}