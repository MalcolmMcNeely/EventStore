using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.InMemory.Events.Streams;
using EventStore.InMemory.Projections;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.SimpleTestDomain;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Core.Tests.ProjectionBuilderTests;

public class ProjectionBuilderRegistrationTests
{
    ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = new ServiceCollection()
            .AddTransient<IProjectionRepository<TestProjection>, ProjectionRepository<TestProjection>>()
            .AddTransient<ProjectionBuilder<TestProjection>, TestDefaultKeyProjectionBuilder>()
            .AddTransient<IProjection, TestProjection>()
            .AddSingleton<IEventStreamFactory, NullEventStreamFactory>()
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() => _serviceProvider.Dispose();

    [Test]
    public void it_can_resolve_a_projection_builder()
    {
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var registration = new ProjectionBuilderRegistration(scopeFactory);
        var projections = registration.ProjectionBuildersFor(typeof(TestEvent));

        Assert.That(projections, Is.Not.Null);
    }
}