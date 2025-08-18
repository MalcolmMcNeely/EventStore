using EventStore.AggregateRoots;
using EventStore.SampleApp.Domain.Accounts.Commands;
using EventStore.SampleApp.Domain.Accounts.Events;

namespace EventStore.SampleApp.Domain.Accounts.AggregateRoots;

public class Account : AggregateRoot
{
    bool _isClosed;
    AccountModel? _accountModel;

    public Account()
    {
        Handles<AccountOpened>(OnAccountOpened);
        Handles<AccountClosed>(OnAccountClosed);
        Handles<AccountClosed>(_ => { });
        Handles<AccountClosed>(_ => { });
    }

    void OnAccountClosed(AccountClosed @event)
    {
        _isClosed = true;
    }

    void OnAccountOpened(AccountOpened @event)
    {
        _accountModel = @event.AccountModel;
    }

    public Task OpenAccountAsync(OpenAccount command)
    {
        if (_accountModel is not null)
        {
            var newAccount = new AccountModel(command.AccountName, command.Type, Decimal.Zero, command.User);

            Update(new AccountOpened { AccountModel = newAccount, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task CloseAccountAsync(CloseAccount command)
    {
        if (_accountModel is not null && !_isClosed)
        {
            Update(new AccountClosed { AccountModel = _accountModel!, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task CreditAccountAsync(CreditAccount command)
    {
        if (_accountModel is not null && !_isClosed)
        {
            Update(new AccountCredited { AccountName = command.AccountName, Amount = command.Amount, User = command.User });
        }

        return Task.CompletedTask;
    }

    public Task DebitAccountAsync(DebitAccount command)
    {
        if (_accountModel is not null && !_isClosed)
        {
            Update(new AccountDebited { AccountName = command.AccountName, Amount = command.Amount, User = command.User });
        }

        return Task.CompletedTask;
    }
}