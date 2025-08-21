using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain.Accounts.Events;

namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class TotalBusinessAccountProjectionBuilder : ProjectionBuilder<TotalBusinessAccountProjection>
{
    public TotalBusinessAccountProjectionBuilder(IProjectionRepository<TotalBusinessAccountProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        WithDefaultKey(nameof(TotalBusinessAccountProjection));
        Handles<AccountOpened>(OnAccountOpened);
        Handles<AccountClosed>(OnAccountClosed);
        Handles<AccountCredited>(OnAccountCredited);
        Handles<AccountDebited>(OnAccountDebited);
    }

    void OnAccountDebited(AccountDebited @event, TotalBusinessAccountProjection projection)
    {
        projection.Balance -= @event.Amount;
    }

    void OnAccountCredited(AccountCredited @event, TotalBusinessAccountProjection projection)
    {
        projection.Balance += @event.Amount;
    }

    void OnAccountClosed(AccountClosed @event, TotalBusinessAccountProjection projection)
    {
        projection.Accounts.Remove(@event.AccountName);
    }

    void OnAccountOpened(AccountOpened @event, TotalBusinessAccountProjection projection)
    {
        projection.Accounts.Add(@event.AccountModel.Name);
    }
}