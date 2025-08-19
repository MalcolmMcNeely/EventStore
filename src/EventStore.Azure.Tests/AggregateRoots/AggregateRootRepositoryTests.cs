using EventStore.AggregateRoots;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains.SimpleTestDomain;

namespace EventStore.Azure.Tests.AggregateRoots;

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

        var aggregateRoot = await _repository.LoadAsync("test");

        await Verify(aggregateRoot);
    }

    [Test]
    public async Task when_everyone_is_trying_to_update_the_same_aggregate_root()
    {
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(GenerateTasks(2000)));

        var aggregateRoot = await _repository.LoadAsync("test");

        var verifySettings = new VerifySettings();
        verifySettings.ScrubMember("Data");
        await Verify(aggregateRoot, verifySettings);
    }

    IEnumerable<Task> GenerateTasks(int numberOfTasks)
    {
        for (var i = 0; i < numberOfTasks; i++)
        {
            yield return _repository.SaveAsync(new TestAggregateRoot { Id = "test", Data = $"{i}" }, "test"); // Write($"{i}");
        }
    }
}