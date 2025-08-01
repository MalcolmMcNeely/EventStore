using EventStore.ProjectionBuilders;

namespace EventStore.Events;

public class EventTypeRegistration(Lazy<IProjectionBuilderRegistration> projectionBuilderRegistration) : IEventTypeRegistration
{
    public Dictionary<string, Type> EventNameToTypeMap => _eventNameToTypeMap.Value;

    readonly Lazy<Dictionary<string, Type>> _eventNameToTypeMap = new(() => CreateEventNameToTypeMap(projectionBuilderRegistration));

    static Dictionary<string, Type> CreateEventNameToTypeMap(Lazy<IProjectionBuilderRegistration> projectionBuilderRegistration)
    {
        var eventNameToTypeMap = new Dictionary<string, Type>();
        var allEventTypes = projectionBuilderRegistration.Value.GetAllEventTypes();

        foreach (var eventType in allEventTypes)
        {
            eventNameToTypeMap[eventType.Name] = eventType;
        }

        return eventNameToTypeMap;
    }
}