using EventStore.Azure.AggegateRoots;
using EventStore.Azure.Events.Streams;
using EventStore.Azure.Events.Transport;
using EventStore.Commands.AggregateRoots;
using EventStore.Events;

namespace EventStore.Azure.Tests.AggregateRoots;

public class AzureAggregateRootRepositoryTests
{
    AzureAggregateRootRepository<TestAggregateRoot> _repository;

    [SetUp]
    public void Setup()
    {
        var azureService = new AzureService();
        var eventStreamFactory = new EventStreamFactory(azureService);
        
        _repository = new AzureAggregateRootRepository<TestAggregateRoot>(azureService, new AzureEventTransport(eventStreamFactory), eventStreamFactory);
    }

    [Test]
    public async Task when_saving_an_aggregate_root()
    {
        await Task.WhenAll(GenerateTasks(1));

        var endValue = await _repository.LoadAsync("test");
        Assert.That(endValue!.Message, Is.EqualTo("0"));
    }

    [Test]
    public async Task when_everyone_is_trying_to_update_the_same_aggregate_root()
    {
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(GenerateTasks(2000)));

        var endValue = await _repository.LoadAsync("test");
        Assert.That(string.IsNullOrWhiteSpace(endValue!.Message), Is.False);
    }

    IEnumerable<Task> GenerateTasks(int numberOfTasks)
    {
        for (var i = 0; i < numberOfTasks; i++)
        {
            yield return Write($"{i}");
        }
    }

    async Task Write(string message)
    {
        await _repository.SaveAsync(new TestAggregateRoot { Message = message }, "test");
    }
}

class TestAggregateRoot : AggregateRoot
{
    public TestAggregateRoot()
    {
        NewEvents.Add((typeof(TestAggregateRootEvent), new TestAggregateRootEvent()));
    }
    
    public string Message { get; set; } = string.Empty;
}

class TestAggregateRootEvent : IEvent;