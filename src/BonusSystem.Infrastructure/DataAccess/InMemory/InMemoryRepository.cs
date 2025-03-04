using BonusSystem.Core.Repositories;
using System.Collections.Concurrent;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// Base repository implementation using in-memory collections
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public abstract class InMemoryRepository<T, TKey> : IRepository<T, TKey>
    where T : class
    where TKey : notnull
{
    protected readonly ConcurrentDictionary<TKey, T> _entities = new();
    protected readonly Func<T, TKey> _keySelector;

    protected InMemoryRepository(Func<T, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public virtual Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_entities.Values);
    }

    public virtual Task<T?> GetByIdAsync(TKey id)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public virtual Task<TKey> CreateAsync(T entity)
    {
        var key = _keySelector(entity);
        _entities[key] = entity;
        return Task.FromResult(key);
    }

    public virtual Task<bool> UpdateAsync(T entity)
    {
        var key = _keySelector(entity);
        return Task.FromResult(_entities.TryUpdate(key, entity, _entities[key]));
    }

    public virtual Task<bool> DeleteAsync(TKey id)
    {
        return Task.FromResult(_entities.TryRemove(id, out _));
    }
}