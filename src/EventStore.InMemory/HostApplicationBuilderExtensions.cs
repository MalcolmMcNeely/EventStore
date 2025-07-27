using EventStore.Commands.AggregateRoots;
using EventStore.Core.Tests.Commands.Transactions;
using EventStore.Events.Transport;
using EventStore.InMemory.Projections;
using EventStore.InMemory.Transport;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.InMemory;

public static class HostApplicationBuilderExtensions
{
    public static void AddDefaultServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(InMemoryAggregateRootRepository<>));
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(InMemoryProjectionRepository<>));
        hostBuilder.Services.AddSingleton<IEventTransport, InMemoryEventTransport>();
    }
}