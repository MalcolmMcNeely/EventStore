using Azure.Data.Tables;
using EventStore.Azure.Azure;
using EventStore.Azure.Events;
using EventStore.Azure.Extensions;
using EventStore.Events;
using EventStore.Events.Streams;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.Utility;

namespace EventStore.Azure.Tests.Events.Streams;

public class EventStreamTests : IntegrationTest
{
    TableClient _eventStoreTable;
    IEventStreamFactory _eventStreamFactory;

    [OneTimeSetUp]
    public void Configure()
    {
        TestConfiguration
            .Configure()
            .WithAzureServices()
            .Build();
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

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new EventStreamTestEvent()), 1);

        var metadataEntity = await _eventStoreTable.GetMetadataEntityAsync(streamName);
        var events = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(metadataEntity.LastEvent, Is.EqualTo(1));
            Assert.That(events.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var streamName = "anotherStream";

        await TestUtility.InvokeManyAsync(async () => await _eventStreamFactory.For(streamName).PublishAsync(new EventStreamTestEvent()), 2000);

        var metadataEntity = await _eventStoreTable.GetMetadataEntityAsync(streamName);
        var events = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(metadataEntity.LastEvent, Is.EqualTo(2000));
            Assert.That(events.Count, Is.EqualTo(2001));
        });
    }

    class EventStreamTestEvent : IEvent
    {
        public string CausationId { get; set; }
    }
}