using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.SimpleTestDomain;

namespace EventStore.Azure.Tests.Projections;

public class ProjectionBuilderTests : IntegrationTest
{
    IProjectionRepository<TestProjection> _projectionRepository;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
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
        await SendEventAsync(new TestEvent { Data = "test" });

        var projection = await _projectionRepository.LoadAsync(nameof(TestProjection));

        Assert.That(projection, Is.Not.Null);
        Assert.That(projection.Data, Is.EqualTo("test"));
    }
}