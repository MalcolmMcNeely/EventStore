using System.Reflection;
using EventStore.Commands.AggregateRoots;
using EventStore.EFCore.Postgres.AggregateRoots;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.EFCore.Postgres.Projections;
using EventStore.Events.Streams;
using EventStore.Events.Transport;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventStore.EFCore.Postgres;

public static class HostBuilderInstaller
{
    public static void AddEFServices(this IHostApplicationBuilder hostBuilder, string connectionString, params Assembly[] aggregateAssemblies)
    {
        hostBuilder.Services.AddDbContext<EventStoreDbContext>(options => { options.UseNpgsql(connectionString); });

        hostBuilder.Services.AddScoped(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<EventStoreDbContext>>();
            return new EventStoreDbContext(options, aggregateAssemblies);
        });

        hostBuilder.Services.AddScoped(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        hostBuilder.Services.AddScoped<EventCursorFactory>();
        hostBuilder.Services.AddScoped<IEventStreamFactory, EventStreamFactory>();

        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddScoped<IEventTransport, EventTransport>();

        hostBuilder.Services.AddScoped<ProjectionRebuilder>();
        hostBuilder.Services.AddScoped(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }
}