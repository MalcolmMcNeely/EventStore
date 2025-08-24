using EventStore.EFCore.Postgres.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EventStore.SampleApp.Domain;

public class DbContextFactory : IDesignTimeDbContextFactory<EventStoreDbContext>
{
    const string ConnectionString = "Server=localhost;Port=5432;Database=EventStore;User ID=postgres;Password=password;Include Error Detail=true;";
    
    public EventStoreDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        var connectionString = configuration.GetConnectionString("Postgres");

        var optionsBuilder = new DbContextOptionsBuilder<EventStoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(DbContextFactory).Assembly.GetName().Name));

        var dbContextAssemblyProvider = new DbContextAssemblyProvider
        {
            AggregateAssemblies = new[]
            {
                typeof(AppDomainNamespace).Assembly,
                typeof(EventStoreDbContext).Assembly
            }
        };

        return new EventStoreDbContext(optionsBuilder.Options, dbContextAssemblyProvider);
    }
}