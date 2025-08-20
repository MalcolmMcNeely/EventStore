using EventStore.Projections;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.MultiStreamProjection;

namespace EventStore.EFCore.Postgres.Tests.Projections;

public class MultiStreamProjectionBuilderTests : PostgresIntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(ProjectionBuilderTests).Assembly)
            .WithMultiStreamProjectionDomain(true)
            .Build();
    }

    [Test]
    public async Task when_command_is_dispatched_it_updates_both_projections()
    {
        await DispatchCommandAsync(new MultiStreamProjectionCommand { Stream = "testStream", Data = "test" });

        var firstProjection = await GetScopedService<IProjectionRepository<FirstKeyedProjection>>().LoadAsync("testStream");
        var secondProjection = await GetScopedService<IProjectionRepository<SecondKeyedProjection>>().LoadAsync("testStream");

        await Verify(new
        {
            First = firstProjection,
            Second = secondProjection
        });
    }
}