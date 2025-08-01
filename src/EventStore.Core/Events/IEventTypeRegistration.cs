namespace EventStore.Events;

public interface IEventTypeRegistration
{
    public Dictionary<string, Type> EventNameToTypeMap { get; }
}