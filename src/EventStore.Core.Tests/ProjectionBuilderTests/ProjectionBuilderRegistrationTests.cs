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
            .AddSingleton<IProjectionRepository<ProjectionBuilderRegistrationTestProjection>, InMemoryProjectionRepository<ProjectionBuilderRegistrationTestProjection>>()
            .AddTransient<IProjectionBuilder, ProjectionBuilderRegistrationTestProjectionBuilder>()
            .BuildServiceProvider();
    }

    [Test]
    public void it_can_resolve_a_projection_builder()
    {
        _registration = new(_serviceProvider);
    }

    [TearDown]
    public void TearDown() => _serviceProvider.Dispose();

    class ProjectionBuilderRegistrationTestEvent1 : IEvent
    {
        public required string Message { get; set; }
    }
    
    class ProjectionBuilderRegistrationTestProjection : IProjection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Message { get; set; }
    }

    class ProjectionBuilderRegistrationTestProjectionBuilder : ProjectionBuilder<ProjectionBuilderRegistrationTestProjection>
    {
        public ProjectionBuilderRegistrationTestProjectionBuilder(IProjectionRepository<ProjectionBuilderRegistrationTestProjection> repository) : base(repository)
        {
            Handles<ProjectionBuilderRegistrationTestEvent1>(OnEvent);
        }

        void OnEvent(ProjectionBuilderRegistrationTestEvent1 @event, ProjectionBuilderRegistrationTestProjection projection)
        {
            projection.Message = @event.Message;
        }
    }
}
