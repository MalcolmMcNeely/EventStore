using EventStore.ProjectionBuilders;
using EventStore.Projections;

namespace EventStore.Azure.Projections;

public class ProjectionRebuilderRegistration(IServiceProvider serviceProvider)
{
    readonly Dictionary<Type, object> _projectionBuilderMap = new();
    
    public ProjectionBuilder<T> ProjectionBuilderFor<T>() where T : IProjection, new()
    {
        if (!_projectionBuilderMap.ContainsKey(typeof(T)))
        {
            var projectionBuilderType = typeof(ProjectionBuilder<>).MakeGenericType(typeof(T));
            var projectionBuilder = serviceProvider.GetService(projectionBuilderType);

            if (projectionBuilder is null)
            {
                throw new ProjectionBuilderRegistrationException($"Projection builder {typeof(T)} not found.");
            }
            
            _projectionBuilderMap.Add(projectionBuilder.GetType(), projectionBuilder);
        }
            
        return (ProjectionBuilder<T>)_projectionBuilderMap[typeof(T)];
    }
}