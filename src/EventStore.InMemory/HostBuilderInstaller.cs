using EventStore.Commands.AggregateRoots;
using EventStore.Events.Transport;
using EventStore.InMemory.AggregateRoots;
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
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(InMemoryAggregateRootRepository<>));
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(InMemoryProjectionRepository<>));
        hostBuilder.Services.AddSingleton<IEventBroadcaster, InMemoryEventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, InMemoryEventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, InMemoryEventTransport>();
    }
}