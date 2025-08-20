using EventStore.AggregateRoots;
using EventStore.Testing;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Core.Tests.AggregateRoots;

public class AggregateRootRepositoryTests : IntegrationTest
{
    IAggregateRootRepository<SimpleAggregateRoot> _repository;

    [SetUp]
    public void Setup()
    {
        _repository = GetService<IAggregateRootRepository<SimpleAggregateRoot>>();
    }

    [Test]
    public async Task when_saving_an_aggregate_root()
    {
        await Task.WhenAll(GenerateTasks(1));

        var aggregateRoot = await _repository.LoadAsync("test");

        await Verify(aggregateRoot);
    }

    [Test]
    public async Task when_everyone_is_trying_to_update_the_same_aggregate_root()
    {
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(GenerateTasks(2000)));

        var aggregateRoot = await _repository.LoadAsync("test");

        await Verify(aggregateRoot);
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
        await _repository.SaveAsync(new SimpleAggregateRoot { Id = "test", Data = message }, "test");
    }
}