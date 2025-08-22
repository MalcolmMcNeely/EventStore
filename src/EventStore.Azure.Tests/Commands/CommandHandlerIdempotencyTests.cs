using EventStore.AggregateRoots;
using EventStore.Projections;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Idempotency;

namespace EventStore.Azure.Tests.Commands;

public class CommandHandlerIdempotencyTests : IntegrationTest
{
    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .WithIdempotencyDomain()
            .Build();
    }

    [Test]
    public async Task commands_can_be_idempotent()
    {
        await DispatchCommandAsync(new IdempotencyCommand { Stream = "testStream", Data = "data" });
        await DispatchCommandAsync(new IdempotencyCommand { Stream = "testStream", Data = "data2" });

        var aggregateRoot = await GetService<IAggregateRootRepository<IdempotencyAggregateRoot>>().LoadAsync("testStream");
        var projection = await GetService<IProjectionRepository<IdempotencyProjection>>().LoadAsync("testStream");

        await Verify(new { aggregateRoot, projection });
    }
}