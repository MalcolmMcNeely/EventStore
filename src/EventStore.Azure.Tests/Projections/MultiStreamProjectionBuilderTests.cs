using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.MultiStreamProjection;

namespace EventStore.Azure.Tests.Projections;

public class MultiStreamProjectionBuilderTests : IntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithMultiStreamProjectionDomain()
            .Build();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_both_projections()
    {
        await DispatchCommandAsync(new MultiStreamProjectionCommand { Stream = "testStream", Data = "test" });

        var firstProjection = await GetService<IProjectionRepository<FirstKeyedProjection>>().LoadAsync("testStream");
        var secondProjection = await GetService<IProjectionRepository<SecondKeyedProjection>>().LoadAsync("testStream");

        await Verify(new
        {
            First = firstProjection,
            Second = secondProjection
        });
    }
}