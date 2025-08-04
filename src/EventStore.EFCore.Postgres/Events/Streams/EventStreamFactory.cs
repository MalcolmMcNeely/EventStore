using System.Collections.Concurrent;
using EventStore.Events.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.EFCore.Postgres.Events.Streams;

public class EventStreamFactory(IServiceScopeFactory serviceScopeFactory) : IEventStreamFactory
{
    readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlims = new();

    public IEventStream For(string streamName)
    {
        var semaphoreSlim = _semaphoreSlims.GetOrAdd(streamName, _ => new SemaphoreSlim(1, 1));
        return new EventStream(serviceScopeFactory, streamName, semaphoreSlim);
    }
}