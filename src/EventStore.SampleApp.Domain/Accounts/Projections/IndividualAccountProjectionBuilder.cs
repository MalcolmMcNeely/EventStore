using EventStore.Events.Streams;
using EventStore.ProjectionBuilders;
using EventStore.Projections;
using EventStore.SampleApp.Domain.Accounts.Events;

namespace EventStore.SampleApp.Domain.Accounts.Projections;

public class IndividualAccountProjectionBuilder : ProjectionBuilder<IndividualAccountProjection>
{
    public IndividualAccountProjectionBuilder(IProjectionRepository<IndividualAccountProjection> repository, IEventStreamFactory eventStreamFactory) : base(repository, eventStreamFactory)
    {
        Handles<AccountOpened>(x => x.AccountModel.Name, OnAccountOpened);
        Handles<AccountClosed>(x => x.AccountName, OnAccountClosed);
        Handles<AccountCredited>(x => x.AccountName, OnAccountCredited);
        Handles<AccountDebited>(x => x.AccountName, OnAccountDebited);
    }

    void OnAccountDebited(AccountDebited @event, IndividualAccountProjection projection)
    {
        projection.Balance += @event.Amount;
    }

    void OnAccountCredited(AccountCredited @event, IndividualAccountProjection projection)
    {
        projection.Balance -= @event.Amount;
    }

    void OnAccountClosed(AccountClosed @event, IndividualAccountProjection projection)
    {
        projection.IsClosed = true;
    }

    void OnAccountOpened(AccountOpened @event, IndividualAccountProjection projection)
    {
        projection.Name = @event.AccountModel.Name;
    }
}