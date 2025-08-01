using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration(IServiceProvider serviceProvider) : IProjectionBuilderRegistration
{
    readonly Lazy<Dictionary<Type, List<Type>>> _projectionBuilderToEventsTypeMap = new(() => RegisterProjectionBuilders(serviceProvider));
    
    public IEnumerable<Type> ProjectionBuildersFor(Type eventType)
    {
        return _projectionBuilderToEventsTypeMap.Value
            .Where(x => x.Value.Any(x => x.IsAssignableFrom(eventType)))
            .Select(x => x.Key);
    }

    public IEnumerable<Type> GetAllEventTypes()
    {
        return _projectionBuilderToEventsTypeMap.Value
            .SelectMany(x => x.Value.Select(y => y))
            .DistinctBy(x => x.AssemblyQualifiedName);
    }

    static Dictionary<Type, List<Type>> RegisterProjectionBuilders(IServiceProvider serviceProvider)
    {
        var projectionBuilderToEventsTypeMap = new Dictionary<Type, List<Type>>();
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

            projectionBuilderToEventsTypeMap[projectionBuilderType] = eventTypes.ToList();
        }

        return projectionBuilderToEventsTypeMap;
    }
}