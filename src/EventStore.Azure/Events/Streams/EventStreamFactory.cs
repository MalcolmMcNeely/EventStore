using System.Collections.Concurrent;

namespace EventStore.Azure.Events.Streams;

public class EventStreamFactory(AzureService azureService)
{
    readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlims = new();

    public EventStream For(string streamName)
    {
        var semaphoreSlim = _semaphoreSlims.GetOrAdd(streamName, _ => new SemaphoreSlim(1, 1));
        return new EventStream(azureService, streamName, semaphoreSlim);
    }
}