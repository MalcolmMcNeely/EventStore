using Azure.Data.Tables;
using EventStore.Azure.Azure;
using EventStore.Azure.Events;
using EventStore.Azure.Extensions;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.Simple;
using EventStore.Testing.Utility;

namespace EventStore.Azure.Tests.Events.Streams;

public class EventStreamTests : IntegrationTest
{
    TableClient _eventStoreTable;
    IEventStreamFactory _eventStreamFactory;
    readonly VerifySettings _verifySettings = new();

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .Build();

        _verifySettings.ScrubInlineGuids();
        _verifySettings.ScrubMember("Timestamp");
    }

    [SetUp]
    public void Setup()
    {
        var azureService = GetService<AzureService>();
        _eventStoreTable = azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);
        _eventStreamFactory = GetService<IEventStreamFactory>();
    }

    [Test]
    public async Task it_can_publish_an_event()
    {
        var streamName = "oneStream";

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new SimpleEvent()), 1);

        var eventAndMetadataRows = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        await Verify(eventAndMetadataRows, _verifySettings);
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var streamName = "anotherStream";

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new SimpleEvent()), 2000);

        var eventAndMetadataRows = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        await Verify(eventAndMetadataRows, _verifySettings);
    }
}