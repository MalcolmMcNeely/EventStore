using EventStore.ProjectionBuilders;

namespace EventStore.Events;

public class EventTypeRegistration
{
    public Dictionary<string, Type> EventNameToTypeMap { get; } = new();

    public EventTypeRegistration(ProjectionBuilderRegistration projectionBuilderRegistration)
    {
        RegisterEventTypes(projectionBuilderRegistration);
    }

    void RegisterEventTypes(ProjectionBuilderRegistration projectionBuilderRegistration)
    {
        var allEventTypes = projectionBuilderRegistration.GetAllEventTypes();

        foreach (var eventType in allEventTypes)
        {
            EventNameToTypeMap[eventType.Name] = eventType;
        }
    }
}