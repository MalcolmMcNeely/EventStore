using EventStore.AggregateRoots;
using EventStore.SampleApp.Domain.Accounts.Commands;
using EventStore.SampleApp.Domain.Accounts.Events;

namespace EventStore.SampleApp.Domain.Accounts.AggregateRoots;

public class Account : AggregateRoot
{
    public bool IsClosed { get; set; }
    public AccountModel? AccountModel { get; set; }

    public Account()
    {
        Handles<AccountOpened>(OnAccountOpened);
        Handles<AccountClosed>(OnAccountClosed);
        Handles<AccountCredited>(_ => { });
        Handles<AccountDebited>(_ => { });
    }

    void OnAccountClosed(AccountClosed @event)
    {
        IsClosed = true;
    }

    void OnAccountOpened(AccountOpened @event)
    {
        AccountModel = @event.AccountModel;
    }

    public Task OpenAccountAsync(OpenAccount command)
    {
        if (AccountModel is null)
        {
            var newAccount = new AccountModel
            {
                Name = command.AccountName,
                Type = command.Type,
                CreatedBy = command.User
            };

            Update(new AccountOpened { AccountModel = newAccount, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task CloseAccountAsync(CloseAccount command)
    {
        if (AccountModel is not null && !IsClosed)
        {
            Update(new AccountClosed { AccountName = AccountModel.Name, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task CreditAccountAsync(CreditAccount command)
    {
        if (AccountModel is not null && !IsClosed)
        {
            Update(new AccountCredited { AccountName = command.AccountName, Amount = command.Amount, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task DebitAccountAsync(DebitAccount command)
    {
        if (AccountModel is not null && !IsClosed)
        {
            Update(new AccountDebited { AccountName = command.AccountName, Amount = command.Amount, User = command.User });
        }

        return Task.CompletedTask;
    }
}