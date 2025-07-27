using EventStore.Events;
using EventStore.InMemory.Projections;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Core.Tests.ProjectionBuilderTests;

public class ProjectionBuilderRegistrationTests
{
    ServiceProvider _serviceProvider;
    ProjectionBuilderRegistration _registration;
    
    [SetUp]
    public void Setup()
    {
        _serviceProvider = new ServiceCollection()
            .AddTransient<IProjectionRepository<ProjectionBuilderRegistrationTestProjection>, InMemoryProjectionRepository<ProjectionBuilderRegistrationTestProjection>>()
            .AddTransient<IProjectionBuilder, ProjectionBuilderRegistrationTestProjectionBuilder>()
            .BuildServiceProvider();
    }

    [Test]
    public void it_can_resolve_a_projection_builder()
    {
        _registration = new(_serviceProvider);
        var projections = _registration.ProjectionsFor(typeof(ProjectionBuilderRegistrationTestEvent));
        
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
        public ProjectionBuilderRegistrationTestProjectionBuilder(IProjectionRepository<ProjectionBuilderRegistrationTestProjection> repository) : base(repository)
        {
            Handles<ProjectionBuilderRegistrationTestEvent>(OnEvent);
        }

        void OnEvent(ProjectionBuilderRegistrationTestEvent @event, ProjectionBuilderRegistrationTestProjection projection)
        {
            projection.Message = @event.Message;
        }
    }
}
