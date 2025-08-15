using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration(IServiceScopeFactory scopeFactory) : IProjectionBuilderRegistration
{
    readonly Lazy<Dictionary<Type, List<Type>>> _projectionBuilderToEventsTypeMap = new(() => RegisterProjectionBuilders(scopeFactory));
    
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

    static Dictionary<Type, List<Type>> RegisterProjectionBuilders(IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var projectionBuilderToEventsTypeMap = new Dictionary<Type, List<Type>>();
        var projectionTypes = scopedProvider.GetServices<IProjection>().Select(x => x.GetType());

        foreach (var projectionType in projectionTypes)
        {
            var projectionBuilderType = typeof(ProjectionBuilder<>).MakeGenericType(projectionType);
            var projectionBuilder = scopedProvider.GetService(projectionBuilderType);

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