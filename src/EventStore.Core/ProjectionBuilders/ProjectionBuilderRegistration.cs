using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration
{
    Dictionary<Type, List<Type>> ProjectionBuilderToEventsTypeMap = new();
    
    public ProjectionBuilderRegistration(IServiceProvider serviceProvider)
    {
        RegisterProjectionBuilders(serviceProvider);
    }

    public IEnumerable<Type> ProjectionBuildersFor(Type eventType)
    {
        return ProjectionBuilderToEventsTypeMap
            .Where(x => x.Value.Any(x => x.IsAssignableFrom(eventType)))
            .Select(x => x.Key);
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
                throw new Exception($"ProjectionBuilder {projectionType} not found.");
            }
            
            var getEventTypesMethod = projectionBuilderType.GetMethod("GetEventTypes");
            var eventTypes = (IEnumerable<Type>)getEventTypesMethod!.Invoke(projectionBuilder, [])!;

            ProjectionBuilderToEventsTypeMap[projectionBuilderType] = eventTypes.ToList();
        }
    }
}