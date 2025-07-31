using EventStore.Commands;
using EventStore.Commands.AggregateRoots;
using EventStore.Events;
using EventStore.InMemory.AggregateRoots;
using EventStore.InMemory.Events.Transport;

namespace EventStore.Core.Tests.Commands.Transactions;

public class UnitOfWorkTests
{
    AggregateRootRepository<UnitOfWorkTestAggregateRoot> _repository;
    UnitOfWorkTestCommand _command;

    [SetUp]
    public async Task Setup()
    {
        _repository = new AggregateRootRepository<UnitOfWorkTestAggregateRoot>(new EventTransport());
        _command = new UnitOfWorkTestCommand { Message = "Hello" };

        await _repository.CreateUnitOfWork(nameof(UnitOfWorkTestAggregateRoot))
            .PerformAsync(x => x.ChangeStateCommand(_command))
            .CompleteAsync();
    }

    [Test]
    public async Task Test()
    {
        var savedAggregateRoot = await _repository.LoadAsync(nameof(UnitOfWorkTestAggregateRoot));

        Assert.That(_repository.NewEvents.Count, Is.EqualTo(1));
        Assert.That(savedAggregateRoot!.State, Is.EqualTo(_command.Message));
    }

    public class UnitOfWorkTestCommand : ICommand
    {
        public required string Message { get; set; }
    }

    public class UnitOfWorkTestEvent : IEvent
    {
        public required string Message { get; set; }
    }

    public class UnitOfWorkTestAggregateRoot : AggregateRoot
    {
        public string State { get; set; } = string.Empty;

        public UnitOfWorkTestAggregateRoot()
        {
            Handles<UnitOfWorkTestEvent>(OnEvent);
        }

        void OnEvent(UnitOfWorkTestEvent @event)
        {
            State = @event.Message;
        }

        public Task ChangeStateCommand(UnitOfWorkTestCommand command)
        {
            Update(new UnitOfWorkTestEvent {  Message = command.Message });

            return Task.CompletedTask;
        }
    }
}