using Azure.Data.Tables;
using EventStore.Azure.Events.Streams;
using EventStore.Azure.Events.TableEntities;
using EventStore.Azure.Extensions;
using EventStore.Events;
using EventStore.Testing;

namespace EventStore.Azure.Tests.Events.Streams;

public class EventStreamTests
{
    AzureService _azureService;
    TableClient _eventStoreTable;

    [SetUp]
    public void Setup()
    {
        _azureService = new AzureService();
        _eventStoreTable = _azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);
    }

    [Test]
    public async Task it_can_publish_an_event()
    {
        var streamName = "oneStream";
        var tasks = GenerateTasks(streamName, Enumerable.Range(0, 1).Select(x => new EventStreamTestEvent()));
        await Task.WhenAll(tasks);

        var metadataEntity = await _eventStoreTable.GetMetadataEntityAsync(streamName);
        var events = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        Assert.That(metadataEntity.LastEvent, Is.EqualTo(1));
        Assert.That(events.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var streamName = "anotherStream";
        var tasks = GenerateTasks(streamName, Enumerable.Range(0, 2000).Select(x => new EventStreamTestEvent()));
        await Task.WhenAll(tasks);

        var metadataEntity = await _eventStoreTable.GetMetadataEntityAsync(streamName);
        var events = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == streamName).ToListAsync();

        Assert.That(metadataEntity.LastEvent, Is.EqualTo(2000));
        Assert.That(events.Count, Is.EqualTo(2001));
    }

    IEnumerable<Task> GenerateTasks(string streamName, IEnumerable<IEvent> events)
    {
        var eventStream = new EventStreamFactory(_azureService, new Lazy<IEventTypeRegistration>(new TestEventTypeRegistration())).For(streamName);
        return events.Select(e => eventStream.PublishAsync(e));
    }

    class EventStreamTestEvent : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}