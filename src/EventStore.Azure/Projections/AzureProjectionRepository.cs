using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class AzureProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection
{
    public Task<T?> LoadAsync(string id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(T projection, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}