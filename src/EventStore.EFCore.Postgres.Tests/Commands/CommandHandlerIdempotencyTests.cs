using EventStore.AggregateRoots;
using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Idempotency;

namespace EventStore.EFCore.Postgres.Tests.Commands;

public class CommandHandlerIdempotencyTests : IntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithEFCoreServices(typeof(CommandHandlerIdempotencyTests).Assembly)
            .WithIdempotencyDomain()
            .Build();
    }

    [Test]
    public async Task commands_can_be_idempotent()
    {
        await DispatchCommandAsync(new IdempotencyCommand { Stream = "testStream", Data = "data" });
        await DispatchCommandAsync(new IdempotencyCommand { Stream = "testStream", Data = "data2" });

        var aggregateRoot = await GetScopedService<IAggregateRootRepository<IdempotencyAggregateRoot>>().LoadAsync("testStream");
        var projection = await GetScopedService<IProjectionRepository<IdempotencyProjection>>().LoadAsync("testStream");

        await Verify(new { First = aggregateRoot, Second = projection }).ScrubMember("RowVersion");
    }
}