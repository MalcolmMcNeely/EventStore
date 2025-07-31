namespace EventStore.Azure.Events.Transport;

public class EventBroadcasterException(string message) : Exception(message);