namespace EventStore.ProjectionBuilders;

public interface IProjectionBuilderRegistration
{
    IEnumerable<Type> ProjectionBuildersFor(Type eventType);
    IEnumerable<Type> GetAllEventTypes();
}