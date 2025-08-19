using EventStore.Projections;
using EventStore.Testing.Configuration;
using EventStore.Testing.SimpleTestDomain;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Projections;

public class ProjectionBuilderTests : PostgresIntegrationTest
{
    IProjectionRepository<TestProjection> _projectionRepository;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(ProjectionBuilderTests).Assembly)
            .WithTestDomain()
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        _projectionRepository = GetScopedService<IProjectionRepository<TestProjection>>();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_the_projection()
    {
        await SendEventAsync(new TestEvent { Data = "test" });

        var projection = await _projectionRepository.LoadAsync(nameof(TestProjection));

        Assert.That(projection, Is.Not.Null);
        Assert.That(projection.Data, Is.EqualTo("test"));
    }
}