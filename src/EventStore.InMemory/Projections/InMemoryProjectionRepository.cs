using EventStore.Projections;

namespace EventStore.InMemory.Projections;

public class InMemoryProjectionRepository<T> : IProjectionRepository<T>  where T : IProjection
{
    readonly Dictionary<string, T> _projections = new();
    
    public Task<T?> Load(string id)
    {
        if (_projections.TryGetValue(id, out var projection))
        {
            return Task.FromResult(projection)!;
        }

        return Task.FromResult<T?>(default);
    }

    public Task Save(T projection)
    {
        _projections[projection.Id] = projection;

        return Task.CompletedTask;
    }
}