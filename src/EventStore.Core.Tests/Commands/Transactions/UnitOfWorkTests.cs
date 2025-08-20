using EventStore.AggregateRoots;
using EventStore.Testing;
using EventStore.Testing.Configuration;
using EventStore.Testing.TestDomains;
using EventStore.Testing.TestDomains.Simple;

namespace EventStore.Core.Tests.Commands.Transactions;

public class UnitOfWorkTests : IntegrationTest
{
    IAggregateRootRepository<SimpleAggregateRoot> _repository;
    SimpleCommand _command;
    
    [SetUp]
    public async Task Setup()
    {
        TestConfiguration.Configure().WithSimpleDomain().Build();

        _repository = GetService<IAggregateRootRepository<SimpleAggregateRoot>>();
        _command = new SimpleCommand { Data = "Hello" };

        await _repository.CreateUnitOfWork(nameof(SimpleAggregateRoot), _command)
            .PerformAsync(x => x.OnCommand(_command))
            .CompleteAsync();
    }

    [Test]
    public async Task when_unit_of_work_is_completed()
    {
        var savedAggregateRoot = await _repository.LoadAsync(nameof(SimpleAggregateRoot));

        await Verify(savedAggregateRoot);
    }
}