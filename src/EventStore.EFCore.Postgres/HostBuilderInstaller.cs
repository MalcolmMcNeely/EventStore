using EventStore.Commands.AggregateRoots;
using EventStore.EFCore.Postgres.AggregateRoots;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.EFCore.Postgres.Projections;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.EFCore.Postgres;

public static class HostBuilderInstaller
{
    public static void AddEFServices(this IHostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddSingleton(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        hostBuilder.Services.AddSingleton<EventCursorFactory>();
        hostBuilder.Services.AddSingleton<IEventStreamFactory, EventStreamFactory>();

        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, EventTransport>();

        hostBuilder.Services.AddSingleton<ProjectionRebuilder>();
        //hostBuilder.Services.AddSingleton<ProjectionRebuilderRegistration>();
        hostBuilder.Services.AddSingleton(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }
}