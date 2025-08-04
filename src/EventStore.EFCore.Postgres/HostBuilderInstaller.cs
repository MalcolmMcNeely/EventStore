using System.Reflection;
using System.Text.Json;
using EventStore.Commands.AggregateRoots;
using EventStore.EFCore.Postgres.AggregateRoots;
using EventStore.EFCore.Postgres.Database;
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
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventStore.EFCore.Postgres;

public static class HostBuilderInstaller
{
    public static void AddEFServices(this IHostApplicationBuilder hostBuilder, string connectionString, params Assembly[] aggregateAssemblies)
    {
        hostBuilder.Services.AddScoped<Assembly[]>(_ => aggregateAssemblies);
        hostBuilder.AddDatabase(connectionString, aggregateAssemblies);

        hostBuilder.Services.AddTransient(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        hostBuilder.Services.AddScoped<EventCursorFactory>();
        hostBuilder.Services.AddSingleton<IEventStreamFactory, EventStreamFactory>();

        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, EventTransport>();

        hostBuilder.Services.AddScoped<ProjectionRebuilder>();
        hostBuilder.Services.AddScoped(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }

    static void AddDatabase(this IHostApplicationBuilder hostBuilder, string connectionString, params Assembly[] aggregateAssemblies)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        hostBuilder.Services.AddSingleton(dataSource);

        hostBuilder.Services.AddDbContext<EventStoreDbContext>((serviceProvider, options) =>
        {
            var npgsqlDataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(npgsqlDataSource);
        });

        hostBuilder.Services.AddScoped(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<EventStoreDbContext>>();
            return new EventStoreDbContext(options, aggregateAssemblies);
        });
    }
}