namespace EventStore.Projections;

public interface IProjectionRepository<T> where T : IProjection
{
    Task<T?> LoadAsync(string id, CancellationToken token = default);
    Task SaveAsync(T projection, CancellationToken token = default);
}