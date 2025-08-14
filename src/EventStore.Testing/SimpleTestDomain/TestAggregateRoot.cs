using EventStore.AggregateRoots;

namespace EventStore.Testing.SimpleTestDomain;

public class TestAggregateRoot : AggregateRoot
{
    public string? Data { get; set; }

    public TestAggregateRoot()
    {
        Handles<TestEvent>(OnTestEvent);
    }

    void OnTestEvent(TestEvent @event)
    {
        Data = @event.Data;
    }

    public Task OnCommand(TestCommand command)
    {
        Update(new TestEvent { Data = command.Data });

        return Task.CompletedTask;
    }
}