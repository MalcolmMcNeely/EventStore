using EventStore.AggregateRoots;
using EventStore.Azure.AggegateRoots;
using EventStore.Azure.Events.Cursors;
using EventStore.Azure.Events.Streams;
using EventStore.Azure.Events.Transport;
using EventStore.Azure.Initialization;
using EventStore.Azure.Projections;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.Azure;

public static class HostBuilderInstaller
{
    public static void AddAzureServices(this IHostApplicationBuilder hostBuilder, string connectionString)
    {
        var azureService = new AzureService(connectionString);

        hostBuilder.Services.AddSingleton(azureService);
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        hostBuilder.Services.AddSingleton<EventCursorFactory>();
        hostBuilder.Services.AddSingleton<IEventStreamFactory, EventStreamFactory>();

        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, EventTransport>();

        hostBuilder.Services.AddSingleton<IStorageInitializer, AzureStorageInitializer>();
        hostBuilder.Services.AddSingleton<Storage>();

        hostBuilder.Services.AddSingleton<ProjectionRebuilder>();
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }
}