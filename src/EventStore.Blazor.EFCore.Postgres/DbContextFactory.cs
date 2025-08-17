using EventStore.EFCore.Postgres.Database;
using EventStore.SampleApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventStore.Blazor.EFCore.Postgres;

public class DbContextFactory : IDesignTimeDbContextFactory<EventStoreDbContext>
{
    public EventStoreDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        var connectionString = configuration.GetConnectionString("Postgres");

        var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(DbContextFactory).Assembly.GetName().Name));

        var assemblies = new[]
        {
            typeof(AppDomainNamespace).Assembly,
            typeof(EventStoreDbContext).Assembly
        };

        return new EventStoreDbContext(optionsBuilder.Options, assemblies);
    }

    public void EnsureDatabaseIsMigrated(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(DbContextFactory).Assembly.GetName().Name));

        var assemblies = new[]
        {
            typeof(AppDomainNamespace).Assembly,
            typeof(EventStoreDbContext).Assembly
        };

        var dbContext = new EventStoreDbContext(optionsBuilder.Options, assemblies);
        dbContext.Database.Migrate();
    }
}