using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.InMemory.Projections;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Core.Tests.ProjectionBuilderTests;

public class ProjectionBuilderRegistrationTests
{
    ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = new ServiceCollection()
            .AddTransient<IProjectionRepository<ProjectionBuilderRegistrationTestProjection>, InMemoryProjectionRepository<ProjectionBuilderRegistrationTestProjection>>()
            .AddTransient<ProjectionBuilder<ProjectionBuilderRegistrationTestProjection>, ProjectionBuilderRegistrationTestProjectionBuilder>()
            .AddTransient<IProjection, ProjectionBuilderRegistrationTestProjection>()
            .AddSingleton<IEventStreamFactory, NullEventStreamFactory>()
            .BuildServiceProvider();
    }

    [Test]
    public void it_can_resolve_a_projection_builder()
    {
        var registration = new ProjectionBuilderRegistration(_serviceProvider);
        var projections = registration.ProjectionBuildersFor(typeof(ProjectionBuilderRegistrationTestEvent));

        Assert.That(projections, Is.Not.Null);
    }

    [TearDown]
    public void TearDown() => _serviceProvider.Dispose();

    class ProjectionBuilderRegistrationTestEvent : IEvent
    {
        public required string Message { get; set; }
    }

    class ProjectionBuilderRegistrationTestProjection : IProjection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Message { get; set; }
    }

    class ProjectionBuilderRegistrationTestProjectionBuilder : ProjectionBuilder<ProjectionBuilderRegistrationTestProjection>
    {
        public ProjectionBuilderRegistrationTestProjectionBuilder(IProjectionRepository<ProjectionBuilderRegistrationTestProjection> repository,
            IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
        {
            Handles<ProjectionBuilderRegistrationTestEvent>(OnEvent);
        }

        void OnEvent(ProjectionBuilderRegistrationTestEvent @event, ProjectionBuilderRegistrationTestProjection projection)
        {
            projection.Message = @event.Message;
        }
    }

    class NullEventStreamFactory : IEventStreamFactory
    {
        public IEventStream For(string streamName)
        {
            return new NullEventStream();
        }

        class NullEventStream : IEventStream
        {
            public Task PublishAsync(IEvent entity, CancellationToken token = default)
            {
                return Task.CompletedTask;
            }
        }
    }
}