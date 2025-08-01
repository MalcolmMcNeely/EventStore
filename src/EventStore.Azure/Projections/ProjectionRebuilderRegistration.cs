using System.Collections.Concurrent;
using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRebuilderRegistration(IServiceProvider serviceProvider)
{
    readonly ConcurrentDictionary<Type, object> _projectionBuilderMap = new();

    public ProjectionBuilder<T> ProjectionBuilderFor<T>() where T : IProjection, new()
    {
        if (!_projectionBuilderMap.ContainsKey(typeof(T)))
        {
            var projectionBuilderType = typeof(ProjectionBuilder<>).MakeGenericType(typeof(T));
            var resolvedProjectionBuilder = serviceProvider.GetService(projectionBuilderType);

            if (resolvedProjectionBuilder is null)
            {
                throw new ProjectionBuilderRegistrationException($"Projection builder {typeof(T)} not found in service provider");
            }

            _projectionBuilderMap.TryAdd(typeof(T), resolvedProjectionBuilder);
        }

        _projectionBuilderMap.TryGetValue(typeof(T), out var projectionBuilder);

        if (projectionBuilder is null)
        {
            throw new ProjectionBuilderRegistrationException($"Projection builder {typeof(T)} not added to registration");
        }

        return (ProjectionBuilder<T>)projectionBuilder;
    }
}