using EventStore.Azure.AggegateRoots;
using EventStore.Azure.Projections;
using EventStore.Commands.AggregateRoots;
using EventStore.Events.Transport;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.Azure;

public static class HostApplicationBuilderExtensions
{
    public static void AddInMemoryServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(AzureAggregateRootRepository<>));
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(AzureProjectionRepository<>));
        hostBuilder.Services.AddSingleton<IEventTransport, AzureEventTransport>();
    }
}