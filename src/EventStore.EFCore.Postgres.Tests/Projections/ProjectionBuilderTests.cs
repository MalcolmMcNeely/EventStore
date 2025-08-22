using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Simple;
using NUnit.Framework;

namespace EventStore.EFCore.Postgres.Tests.Projections;

public class ProjectionBuilderTests : IntegrationTest
{
    IProjectionRepository<SimpleProjection> _projectionRepository;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(ProjectionBuilderTests).Assembly)
            .WithSimpleDomain(true)
            .Build();
    }

    [SetUp]
    public void Setup()
    {
        _projectionRepository = GetScopedService<IProjectionRepository<SimpleProjection>>();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_the_projection()
    {
        await DispatchCommandAsync(new SimpleCommand { Data = "test" });

        var projection = await GetScopedService<IProjectionRepository<SimpleProjection>>().LoadAsync(nameof(SimpleProjection));
        
        await Verify(projection);
    }
}