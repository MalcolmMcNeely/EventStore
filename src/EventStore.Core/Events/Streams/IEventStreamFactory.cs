namespace EventStore.Events.Streams;

public interface IEventStreamFactory
{
    IEventStream For(string streamName);
}