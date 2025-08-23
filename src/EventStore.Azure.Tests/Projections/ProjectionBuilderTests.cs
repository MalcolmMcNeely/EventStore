using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Azure.Tests.Projections;

public class ProjectionBuilderTests : IntegrationTest
{
    protected override void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithSimpleDomain()
            .Build();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_the_projection()
    {
        await DispatchCommandAsync(new SimpleCommand { Data = "test" });

        var projection = await GetService<IProjectionRepository<SimpleProjection>>().LoadAsync(nameof(SimpleProjection));

        await Verify(projection);
    }
}