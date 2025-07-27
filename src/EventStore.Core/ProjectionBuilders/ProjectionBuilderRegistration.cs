using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration
{
    Dictionary<Type, List<Type>> ProjectionToEventsTypeMap = new();
    
    public ProjectionBuilderRegistration(IServiceProvider serviceProvider)
    {
        RegisterProjectionBuilders(serviceProvider);
    }

    public IEnumerable<Type> ProjectionsFor(Type eventType)
    {
        return ProjectionToEventsTypeMap
            .Where(x => x.Value.Any(x => x.IsAssignableFrom(eventType)))
            .Select(x => x.Key);
    }

    void RegisterProjectionBuilders(IServiceProvider serviceProvider)
    {
        var projectionBuilders = serviceProvider.GetServices<IProjectionBuilder>().ToList();
        
        foreach (var projectionBuilder in projectionBuilders)
        {
            var baseType = projectionBuilder.GetType().BaseType;

            if (baseType == null || !baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(ProjectionBuilder<>))
            {
                throw new Exception("Invalid ProjectionBuilder base type.");
            }

            var projectionType = baseType.GetGenericArguments()[0];

            if (!typeof(IProjection).IsAssignableFrom(projectionType))
            {
                throw new Exception($"{projectionType} does not implement IProjection.");
            }

            ProjectionToEventsTypeMap[projectionType] = projectionBuilder.GetEventTypes().ToList();
        }
    }
}