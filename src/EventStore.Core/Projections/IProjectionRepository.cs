namespace EventStore.Projections;

public interface IProjectionRepository<T> where T : IProjection
{
    Task<T?> Load(string id);
    Task Save(T projection);
}