﻿using EventStore.Commands.AggregateRoots;
using EventStore.Events;

namespace EventStore.Commands.Transactions;

public class UnitOfWork<T> where T : AggregateRoot, new()
{
    readonly string _key;
    readonly IAggregateRootRepository<T> _aggregateRootRepository;
    readonly List<Func<T, Task>> _actions = new();

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

    public async Task CompleteAsync(CancellationToken token = default)
    {
        var entity = await LoadAndMergeAsync();

        if (!await _aggregateRootRepository.SaveAsync(entity, _key, token))
        {
            var currentRetry = 0;

            while (currentRetry < _retryOptions.MaxRetries)
            {
                if (_retryOptions is ExponentialUnitOfWorkRetryOptions exponentialUnitOfWorkRetryOptions)
                {
                    await Task.Delay(_retryOptions.RetryInterval * currentRetry * exponentialUnitOfWorkRetryOptions.Exponential, token);
                }
                else
                {
                    await Task.Delay(_retryOptions.RetryInterval, token);
                }

                entity = await LoadAndMergeAsync();

                if (await _aggregateRootRepository.SaveAsync(entity, _key, token))
                {
                    break;
                }

                if (currentRetry == _retryOptions.MaxRetries)
                {
                    throw new UnitOfWorkException($"Maximum number of retries reached trying to update {typeof(T).Name}");
                }

                currentRetry++;
            }
        }

        if (entity.NewEvents.Count != 0)
        {
            foreach (var (_, @event) in entity.NewEvents)
            {
                await _aggregateRootRepository.SendEventAsync(@event, _key, token);
            }

            entity.NewEvents.Clear();
        }
    }

    async Task<T> LoadAndMergeAsync()
    {
        var entity = await _aggregateRootRepository.LoadAsync(_key) ?? new T();

        foreach (var action in _actions)
        {
            await action(entity);
        }

        entity.ApplyUpdates();

        return entity;
    }
}