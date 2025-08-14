using EventStore.AggregateRoots;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using EventStore.InMemory.AggregateRoots;
using EventStore.InMemory.Events.Streams;
using EventStore.InMemory.Events.Transport;
using EventStore.InMemory.Projections;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.InMemory;

public static class HostBuilderInstaller
{
    public static void AddInMemoryServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));
        hostBuilder.Services.AddSingleton<IEventStreamFactory, NullEventStreamFactory>();
        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, EventTransport>();
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }
}