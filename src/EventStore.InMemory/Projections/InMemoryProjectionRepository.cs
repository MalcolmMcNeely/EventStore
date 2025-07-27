using EventStore.Projections;

namespace EventStore.InMemory.Projections;

public class InMemoryProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection
{
    readonly Dictionary<string, T> _projections = new();
    
    public Task<T?> LoadAsync(string id, CancellationToken _ = default)
    {
        if (_projections.TryGetValue(id, out var projection))
        {
            return Task.FromResult(projection)!;
        }

        return Task.FromResult<T?>(default);
    }

    public Task SaveAsync(T projection, CancellationToken _ = default)
    {
        _projections[projection.Id] = projection;

        return Task.CompletedTask;
    }
}