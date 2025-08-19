using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventStore.EFCore.Postgres;

public class PostgresOptions
{
    public string ConnectionString { get; set; }
    public Assembly? MigrationsAssembly { get; set; }
    public bool AutoMigrate { get; set; }
    public Assembly[] AggregateAssemblies { get; set; }

    public void DetermineMigrationsAssembly()
    {
        if (MigrationsAssembly is not null)
        {
            return;
        }

        MigrationsAssembly ??= AggregateAssemblies.FirstOrDefault(a => a.GetTypes().Any(t => typeof(Migration).IsAssignableFrom(t))) ?? AggregateAssemblies.FirstOrDefault();

        if (MigrationsAssembly is null)
        {
            throw new StartupException("Could not find migrations assembly");
        }
    }
}