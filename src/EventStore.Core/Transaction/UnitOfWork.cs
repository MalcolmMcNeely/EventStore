using EventStore.Commands.AggregateRoot;

namespace EventStore.Transaction;

public class UnitOfWork<T> where T : AggregateRoot, new()
{
    readonly string _key;
    readonly IAggregateRootRepository<T> _aggregateRootRepository;
    readonly List<Func<T, Task>> _actions = new();
    readonly int _currentRetry = 0;

    UnitOfWorkRetryOptions _retryOptions = new(TimeSpan.FromSeconds(1), 3);

    internal UnitOfWork(string key, IAggregateRootRepository<T> aggregateRootRepository)
    {
        _key = key;
        _aggregateRootRepository = aggregateRootRepository;
    }

    public UnitOfWork<T> PerformAsync(Func<T, Task> action)
    {
        _actions.Add(action);

        return this;
    }

    public UnitOfWork<T> WithRetry(UnitOfWorkRetryOptions retryOptions)
    {
        _retryOptions = retryOptions;

        return this;
    }

    public async Task CompleteAsync()
    {
        var entity = await LoadAndMergeAsync();

        if (!await _aggregateRootRepository.SaveAsync(entity, _key))
        {
            while (_currentRetry < _retryOptions.MaxRetries)
            {
                if (_retryOptions is ExponentialUnitOfWorkRetryOptions exponentialUnitOfWorkRetryOptions)
                {
                    await Task.Delay(_retryOptions.RetryInterval * _currentRetry * exponentialUnitOfWorkRetryOptions.Exponential);
                }
                else
                {
                    await Task.Delay(_retryOptions.RetryInterval);
                }

                var retryEntity = await LoadAndMergeAsync();

                if (await _aggregateRootRepository.SaveAsync(entity, _key))
                {
                    break;
                }

                if (_currentRetry == _retryOptions.MaxRetries)
                {
                    throw new Exception();
                }
            }
        }
    }

    async Task<T> LoadAndMergeAsync()
    {
        var entity = await _aggregateRootRepository.LoadAsync(_key) ?? new T();

        foreach (var action in _actions)
        {
            await action(entity);
        }

        return entity;
    }
}