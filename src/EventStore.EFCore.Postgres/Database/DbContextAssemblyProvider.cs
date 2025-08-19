using System.Reflection;

namespace EventStore.EFCore.Postgres.Database;

public class DbContextAssemblyProvider :  IDbContextAssemblyProvider
{
    public Assembly[] AggregateAssemblies { get; init; } = [];
}