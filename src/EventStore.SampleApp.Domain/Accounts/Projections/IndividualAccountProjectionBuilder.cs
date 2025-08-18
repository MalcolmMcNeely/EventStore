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
        Handles<AccountClosed>(x => x.AccountModel.Name, OnAccountClosed);
        Handles<AccountCredited>(x => x.AccountName, OnAccountCredited);
        Handles<AccountDebited>(x => x.AccountName, OnAccountDebited);
    }

    void OnAccountDebited(AccountDebited @event, IndividualAccountProjection projection)
    {
        throw new NotImplementedException();
    }

    void OnAccountCredited(AccountCredited @event, IndividualAccountProjection projection)
    {
        throw new NotImplementedException();
    }

    void OnAccountClosed(AccountClosed @event, IndividualAccountProjection projection)
    {
        throw new NotImplementedException();
    }

    void OnAccountOpened(AccountOpened @event, IndividualAccountProjection projection)
    {
        projection.Name = @event.AccountModel.Name;
    }
}