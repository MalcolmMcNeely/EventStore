using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Azure.Tests.Projections;

public class ProjectionBuilderTests : IntegrationTest
{
    IProjectionRepository<SimpleProjection> _projectionRepository;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithSimpleDomain()
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        _projectionRepository = GetService<IProjectionRepository<SimpleProjection>>();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_the_projection()
    {
        await DispatchCommandAsync(new SimpleCommand { Data = "test" });

        var projection = await _projectionRepository.LoadAsync(nameof(SimpleProjection));

        await Verify(projection);
    }
}