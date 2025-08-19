using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.SimpleTestDomain;

namespace EventStore.Core.Tests.ProjectionBuilderTests;

public class ProjectionBuilderTests : IntegrationTest
{
    IProjectionRepository<TestProjection>? _projectionRepository;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithInMemoryServices()
            .WithSimpleTestDomain()
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        _projectionRepository = GetService<IProjectionRepository<TestProjection>>();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_the_projection()
    {
        Assert.That(_projectionRepository, Is.Not.Null);

        await SendEventAsync(new TestEvent { Data = "test" });

        var projection = await _projectionRepository.LoadAsync(nameof(TestProjection));

        await Verify(projection);
    }
}