using System.Reflection;
using EventStore.Commands.AggregateRoots;
using EventStore.EFCore.Postgres.Events.Cursors;
using EventStore.EFCore.Postgres.Events.Streams;
using EventStore.EFCore.Postgres.Events.Transport;
using EventStore.Projections;
using Microsoft.EntityFrameworkCore;

namespace EventStore.EFCore.Postgres;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> options, params Assembly[] aggregateAssemblies) : DbContext(options)
{
    public DbSet<EventCursorEntity> EventCursorEntities { get; set; }
    public DbSet<EventStreamEntity> EventStreams { get; set; }
    public DbSet<QueuedEventEntity> QueuedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var aggregateTypes = aggregateAssemblies.SelectMany(x => x
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(AggregateRoot).IsAssignableFrom(t)));

        foreach (var type in aggregateTypes)
        {
            modelBuilder.Entity(type).ToTable(type.Name + "s");
        }

        var projectionTypes = aggregateAssemblies.SelectMany(x => x
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IProjection).IsAssignableFrom(t)));

        foreach (var type in projectionTypes)
        {
            modelBuilder.Entity(type).ToTable(type.Name + "s");
        }

        base.OnModelCreating(modelBuilder);
    }
}