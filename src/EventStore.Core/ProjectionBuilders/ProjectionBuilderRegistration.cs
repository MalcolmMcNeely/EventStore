using Microsoft.Extensions.DependencyInjection;

namespace EventStore.ProjectionBuilders;

public class ProjectionBuilderRegistration
{
    Dictionary<Type, List<Type>> ProjectionBuilderToEventsTypeMap = new();
    
    public ProjectionBuilderRegistration(IServiceProvider serviceProvider)
    {
        RegisterProjectionBuilders(serviceProvider);
    }

    void RegisterProjectionBuilders(IServiceProvider serviceProvider)
    {
        var projectionBuilders = serviceProvider.GetServices<IProjectionBuilder>().ToList();
        
        foreach (var projectionBuilder in projectionBuilders)
        {
            var projectionBuilderType = projectionBuilder.GetType();

            ProjectionBuilderToEventsTypeMap[projectionBuilderType] = projectionBuilder.GetEventTypes().ToList();
        }
    }
}