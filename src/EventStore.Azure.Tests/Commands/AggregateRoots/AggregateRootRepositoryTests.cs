using EventStore.AggregateRoots;
using EventStore.Events;
using EventStore.Testing;
using EventStore.Testing.Configuration;

namespace EventStore.Azure.Tests.Commands.AggregateRoots;

public class AggregateRootRepositoryTests : IntegrationTest
{
    IAggregateRootRepository<TestAggregateRoot> _repository;

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
        _repository = GetService<IAggregateRootRepository<TestAggregateRoot>>();
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
    
    class TestAggregateRoot : AggregateRoot
    {
        public TestAggregateRoot()
        {
            NewEvents.Add((typeof(TestAggregateRootEvent), new TestAggregateRootEvent()));
        }
    
        public string Message { get; set; } = string.Empty;
    }

    class TestAggregateRootEvent : IEvent
    {
        public string CausationId { get; set; }
    }
}