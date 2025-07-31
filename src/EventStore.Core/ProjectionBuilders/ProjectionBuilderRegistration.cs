using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration
{
    readonly Dictionary<Type, List<Type>> _projectionBuilderToEventsTypeMap = new();
    
    public ProjectionBuilderRegistration(IServiceProvider serviceProvider)
    {
        RegisterProjectionBuilders(serviceProvider);
    }

    public IEnumerable<Type> ProjectionBuildersFor(Type eventType)
    {
        return _projectionBuilderToEventsTypeMap
            .Where(x => x.Value.Any(x => x.IsAssignableFrom(eventType)))
            .Select(x => x.Key);
    }

    public IEnumerable<Type> GetAllEventTypes()
    {
        return _projectionBuilderToEventsTypeMap
            .SelectMany(x => x.Value.Select(y => y))
            .DistinctBy(x => x.AssemblyQualifiedName);
    }

    void RegisterProjectionBuilders(IServiceProvider serviceProvider)
    {
        var projectionTypes = serviceProvider.GetServices<IProjection>().Select(x => x.GetType());

        foreach (var projectionType in projectionTypes)
        {
            var projectionBuilderType = typeof(ProjectionBuilder<>).MakeGenericType(projectionType);
            var projectionBuilder = serviceProvider.GetService(projectionBuilderType);

            if (projectionBuilder is null)
            {
                throw new ProjectionBuilderRegistrationException($"Projection builder {projectionType} not found.");
            }
            
            var getEventTypesMethod = projectionBuilderType.GetMethod("GetEventTypes");
            var eventTypes = (IEnumerable<Type>)getEventTypesMethod!.Invoke(projectionBuilder, [])!;

            _projectionBuilderToEventsTypeMap[projectionBuilderType] = eventTypes.ToList();
        }
    }
}