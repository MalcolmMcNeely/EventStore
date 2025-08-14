namespace EventStore.Commands;

public interface ICommand
{
    public string CausationId { get; set; }
}