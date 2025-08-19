using System.Reflection;
using EventStore.AggregateRoots;
using EventStore.Commands.Dispatching;
using EventStore.EFCore.Postgres.AggregateRoots;
using EventStore.EFCore.Postgres.Commands;
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
using Npgsql;

namespace EventStore.EFCore.Postgres;

public static class HostBuilderInstaller
{
    public static void AddPostgresServices(this IHostApplicationBuilder hostBuilder, Action<PostgresOptions> configureOptions)
    {
        var options = new PostgresOptions();
        configureOptions.Invoke(options);
        options.DetermineMigrationsAssembly();

        var dbContextAssemblyProvider = new DbContextAssemblyProvider { AggregateAssemblies = options.AggregateAssemblies, };
        hostBuilder.Services.AddScoped<IDbContextAssemblyProvider, DbContextAssemblyProvider>(_ => dbContextAssemblyProvider);
        hostBuilder.AddDatabase(options, dbContextAssemblyProvider);

        if (options.AutoMigrate)
        {
            EnsureDatabaseIsMigrated(options, dbContextAssemblyProvider);
        }

        hostBuilder.Services.AddTransient(typeof(IAggregateRootRepository<>), typeof(AggregateRootRepository<>));

        hostBuilder.Services.AddSingleton<ICommandAudit, CommandAudit>();

        hostBuilder.Services.AddScoped<EventCursorFactory>();
        hostBuilder.Services.AddSingleton<IEventStreamFactory, EventStreamFactory>();

        hostBuilder.Services.AddSingleton<IEventBroadcaster, EventBroadcaster>();
        hostBuilder.Services.AddSingleton<IEventPump, EventPump>();
        hostBuilder.Services.AddSingleton<IEventTransport, EventTransport>();

        hostBuilder.Services.AddScoped<ProjectionRebuilder>();
        hostBuilder.Services.AddScoped(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));
    }

    static void AddDatabase(this IHostApplicationBuilder hostBuilder, PostgresOptions options, DbContextAssemblyProvider dbContextAssemblyProvider)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(options.ConnectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        hostBuilder.Services.AddSingleton(dataSource);

        hostBuilder.Services.AddDbContext<EventStoreDbContext>((serviceProvider, dbContextOptionsBuilder) =>
        {
            var npgsqlDataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            dbContextOptionsBuilder.UseNpgsql(npgsqlDataSource, npgsql => { npgsql.MigrationsAssembly(options.MigrationsAssembly!); });
        });

        hostBuilder.Services.AddScoped(provider =>
        {
            var dbContextOptions = provider.GetRequiredService<DbContextOptions<EventStoreDbContext>>();
            return new EventStoreDbContext(dbContextOptions, dbContextAssemblyProvider);
        });
    }

    static void EnsureDatabaseIsMigrated(PostgresOptions options, DbContextAssemblyProvider dbContextAssemblyProvider)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
        dbContextOptionsBuilder.UseNpgsql(options.ConnectionString, b => b.MigrationsAssembly(options.MigrationsAssembly!));
        var dbContext = new EventStoreDbContext(dbContextOptionsBuilder.Options, dbContextAssemblyProvider);
        dbContext.Database.Migrate();
    }
}