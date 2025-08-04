using System.Reflection;
using EventStore.Commands.AggregateRoots;
using EventStore.Concurrency;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres.Database;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> options, params Assembly[] aggregateAssemblies) : DbContext(options)
{
    public DbSet<EventCursorEntity> EventCursorEntities { get; set; }
    public DbSet<EventStreamEntity> EventStreams { get; set; }
    public DbSet<QueuedEventEntity> QueuedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventStreamEntity>().HasKey(e => new { e.Key, e.RowKey });

        var aggregateTypes = aggregateAssemblies.SelectMany(x => x
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

        var projectionTypes = aggregateAssemblies.SelectMany(x => x
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