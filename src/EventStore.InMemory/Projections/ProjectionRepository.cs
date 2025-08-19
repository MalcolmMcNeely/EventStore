using System.Collections.Concurrent;
using EventStore.Projections;

namespace EventStore.InMemory.Projections;

public sealed class ProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection, new()
{
    static readonly ConcurrentDictionary<string, T> Projections = new();
    
    public Task<T> LoadAsync(string id, CancellationToken _ = default)
    {
        if (Projections.TryGetValue(id, out var projection))
        {
            return Task.FromResult(projection)!;
        }

        return Task.FromResult(new T { Id = id });
    }

    public Task SaveAsync(T projection, CancellationToken _ = default)
    {
        Projections.TryAdd(projection.Id, projection);

        return Task.CompletedTask;
    }
}