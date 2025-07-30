using Azure.Data.Tables;
using EventStore.Azure.Transport.Events;
using EventStore.Azure.Transport.Events.Streams;
using EventStore.Azure.Transport.Events.TableEntities;
using EventStore.Events;

namespace EventStore.Azure.Tests.Events.Streams;

public class AllStreamTests
{
    AzureService _azureService;
    AllStream _allStream;
    TableClient _eventStoreTable;

    [SetUp]
    public async Task Setup()
    {
        _azureService = new AzureService();
        _allStream = new AllStream(_azureService);

        _eventStoreTable = _azureService.TableServiceClient.GetTableClient(Defaults.Events.EventStoreTable);
        await _eventStoreTable.CreateIfNotExistsAsync();
    }

    [Test]
    public async Task it_can_publish_an_event()
    {
        await _allStream.PublishAsync(Defaults.Streams.AllStreamPartition, new EventStreamTestEvent());
        
        MetadataEntity metadataEntity = await _eventStoreTable.GetEntityAsync<MetadataEntity>(
            Defaults.Streams.AllStreamPartition, 
            RowKey.ForMetadata().ToString());

        var events = await _eventStoreTable.QueryAsync<EventEntity>(
            x => x.PartitionKey == Defaults.Streams.AllStreamPartition)
            .ToListAsync();

        Assert.That(metadataEntity.LastEvent, Is.EqualTo(1));
        Assert.That(events.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task it_can_publish_many_events()
    {
        var tasks = GenerateTasks(Enumerable.Range(0, 2000).Select(x => new EventStreamTestEvent()));
        await Task.WhenAll(tasks);

        MetadataEntity metadataEntity = await _eventStoreTable.GetEntityAsync<MetadataEntity>(Defaults.Streams.AllStreamPartition, RowKey.ForMetadata().ToString());
        var events = await _eventStoreTable.QueryAsync<EventEntity>(x => x.PartitionKey == Defaults.Streams.AllStreamPartition).ToListAsync();
        
        Assert.That(metadataEntity.LastEvent, Is.EqualTo(2000));
        Assert.That(events.Count, Is.EqualTo(2001));
    }

    IEnumerable<Task> GenerateTasks(IEnumerable<IEvent> events)
    {
        return events.Select(e => _allStream.PublishAsync(Defaults.Streams.AllStreamPartition, e));
    }

    class EventStreamTestEvent : IEvent
    {
        public Guid Id { get; set; } =  Guid.NewGuid();
    }
}