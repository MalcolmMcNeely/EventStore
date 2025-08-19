using System.Reflection;
using EventStore.AggregateRoots;
using EventStore.Concurrency;
using EventStore.EFCore.Postgres.Commands;
using EventStore.EFCore.Postgres.Events;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Database;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> options, IDbContextAssemblyProvider assemblyProvider) : DbContext(options)
{
    public DbSet<EventCursorEntity> EventCursorEntities { get; set; }
    public DbSet<EventStreamEntity> EventStreams { get; set; }
    public DbSet<QueuedEventEntity> QueuedEvents { get; set; }
    public DbSet<CommandEntity> Commands { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new SlowQueryInterceptor());

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventStreamEntity>().HasKey(e => new { e.Key, e.RowKey });
        modelBuilder.Entity<CommandEntity>().HasKey(e => new { e.Key, e.RowKey });

        var aggregateTypes = assemblyProvider.AggregateAssemblies.SelectMany(x => x
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(AggregateRoot).IsAssignableFrom(t)));

        foreach (var type in aggregateTypes)
        {
            var entityBuilder =  modelBuilder.Entity(type);
            entityBuilder.ToTable(type.Name + "s");

            var rowVersionProperty = type.GetProperty(nameof(IConcurrencyCheck.RowVersion));
            if (rowVersionProperty != null && rowVersionProperty.PropertyType == typeof(byte[]))
            {
                entityBuilder
                    .Property<int>(nameof(IConcurrencyCheck.RowVersion))
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();
            }
        }

        var projectionTypes = assemblyProvider.AggregateAssemblies.SelectMany(x => x
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IProjection).IsAssignableFrom(t)));

        foreach (var type in projectionTypes)
        {
            var entityBuilder = modelBuilder.Entity(type);
            entityBuilder.ToTable(type.Name + "s");

            var rowVersionProperty = type.GetProperty(nameof(IConcurrencyCheck.RowVersion));
            if (rowVersionProperty != null && rowVersionProperty.PropertyType == typeof(byte[]))
            {
                entityBuilder
                    .Property<int>(nameof(IConcurrencyCheck.RowVersion))
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}