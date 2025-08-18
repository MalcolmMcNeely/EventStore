using EventStore.AggregateRoots;
using EventStore.Commands;
using EventStore.SampleApp.Domain.Accounts.AggregateRoots;

namespace EventStore.SampleApp.Domain.Accounts.Commands;

public class AccountsCommandHandler(IAggregateRootRepository<Account> repository) : 
    ICommandHandler<OpenAccount>, 
    ICommandHandler<CloseAccount>,
    ICommandHandler<CreditAccount>,
    ICommandHandler<DebitAccount>
{
    public async Task HandleAsync(OpenAccount command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(command.AccountName, command)
            .PerformAsync(x => x.OpenAccountAsync(command))
            .CompleteAsync(token);
    }

    public async Task HandleAsync(CloseAccount command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(command.AccountName, command)
            .PerformAsync(x => x.CloseAccountAsync(command))
            .CompleteAsync(token);
    }

    public async Task HandleAsync(CreditAccount command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(command.AccountName, command)
            .PerformAsync(x => x.CreditAccountAsync(command))
            .CompleteAsync(token);
    }

    public async Task HandleAsync(DebitAccount command, CancellationToken token)
    {
        await repository.CreateUnitOfWork(command.AccountName, command)
            .PerformAsync(x => x.DebitAccountAsync(command))
            .CompleteAsync(token);
    }
}