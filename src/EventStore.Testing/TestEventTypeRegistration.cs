using EventStore.Events;

namespace EventStore.Testing;

public class TestEventTypeRegistration : IEventTypeRegistration
{
    public Dictionary<string, Type> EventNameToTypeMap => InternalEventNameToTypeMap;

    static readonly Dictionary<string, Type> InternalEventNameToTypeMap = new();

    public TestEventTypeRegistration WithEvent(Type eventType)
    {
        InternalEventNameToTypeMap[eventType.Name] = eventType;

        return this;
    }

    public TestEventTypeRegistration WithEvents(params Type[] eventTypes)
    {
        foreach (var eventType in eventTypes)
        {
            InternalEventNameToTypeMap[eventType.Name] = eventType;
        }

        return this;
    }
}