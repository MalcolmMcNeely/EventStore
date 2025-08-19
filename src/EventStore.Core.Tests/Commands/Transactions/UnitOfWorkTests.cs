using EventStore.AggregateRoots;
using EventStore.Commands;
using EventStore.Events;
using EventStore.InMemory.AggregateRoots;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.SimpleTestDomain;

namespace EventStore.Core.Tests.Commands.Transactions;

public class UnitOfWorkTests : IntegrationTest
{
    IAggregateRootRepository<TestAggregateRoot> _repository;
    TestCommand _command;
    
    [SetUp]
    public async Task Setup()
    {
        TestConfiguration.Configure().WithSimpleTestDomain().Build();

        _repository = GetService<IAggregateRootRepository<TestAggregateRoot>>();
        _command = new TestCommand { Data = "Hello" };

        await _repository.CreateUnitOfWork(nameof(TestAggregateRoot), _command)
            .PerformAsync(x => x.OnCommand(_command))
            .CompleteAsync();
    }

    [Test]
    public async Task Test()
    {
        var savedAggregateRoot = await _repository.LoadAsync(nameof(TestAggregateRoot));

        Assert.That(savedAggregateRoot!.Data, Is.EqualTo(_command.Data));
    }
}