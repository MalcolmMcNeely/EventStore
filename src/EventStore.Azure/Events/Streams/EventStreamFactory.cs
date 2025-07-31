using System.Collections.Concurrent;
using EventStore.Events.Streams;

namespace EventStore.Azure.Events.Streams;

public class EventStreamFactory(AzureService azureService) : IEventStreamFactory
{
    readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlims = new();

    public IEventStream For(string streamName)
    {
        var semaphoreSlim = _semaphoreSlims.GetOrAdd(streamName, _ => new SemaphoreSlim(1, 1));
        return new EventStream(azureService, streamName, semaphoreSlim);
    }
}