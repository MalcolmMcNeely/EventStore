using System.Collections.Concurrent;
using EventStore.Projections;

namespace EventStore.InMemory.Projections;

public class ProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection
{
    static readonly ConcurrentDictionary<string, T> Projections = new();
    
    public Task<T?> LoadAsync(string id, CancellationToken _ = default)
    {
        if (Projections.TryGetValue(id, out var projection))
        {
            return Task.FromResult(projection)!;
        }

        return Task.FromResult<T?>(default);
    }

    public Task SaveAsync(T projection, CancellationToken _ = default)
    {
        Projections.TryAdd(projection.Id, projection);

        return Task.CompletedTask;
    }
}