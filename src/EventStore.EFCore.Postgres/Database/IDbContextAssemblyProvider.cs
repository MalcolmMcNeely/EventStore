using System.Reflection;

namespace EventStore.EFCore.Postgres.Database;

public interface IDbContextAssemblyProvider
{
    public Assembly[] AggregateAssemblies { get; }
}