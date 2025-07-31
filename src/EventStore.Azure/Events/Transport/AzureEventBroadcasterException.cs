namespace EventStore.Azure.Events.Transport;

public class AzureEventBroadcasterException(string message) : Exception(message);