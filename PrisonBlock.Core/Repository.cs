using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PrisonBlock.Core;

public sealed class Repository<T> : IRepository<T>, IPrisonerEventSource where T : Prisoner
{
    private readonly List<T> _items = new();

    public event Action<Prisoner>? Added;
    public event Action<Prisoner>? Removing;
    public event Action<Prisoner>? CrazyActionPerformed;
    public event Action<string>? LogGenerated;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.Contains(item))
            return;

        _items.Add(item);

        // existing prisoners greet the newcomer 
        Added?.Invoke(item);

        item.ReactionLogged += ForwardLog;
        item.Subscribe(this);

        LogGenerated?.Invoke($"Lisati {item.Name} ({typeof(T).Name})");
    }

    public bool Remove(T item)
    {
        if (item is null || !_items.Contains(item))
            return false;

        // removed prisoner must not react to future events
        item.Unsubscribe(this);
        item.ReactionLogged -= ForwardLog;
        _items.Remove(item);

        // remaining prisoners say goodbye
        Removing?.Invoke(item);

        LogGenerated?.Invoke($"Eemaldati {item.Name}");
        return true;
    }

    public IEnumerable<T> GetAll() => _items.ToList();

    public T? Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _items.FirstOrDefault(predicate);
    }

    public void PublishCrazyAction(Prisoner prisoner)
    {
        if (!_items.Any(item => ReferenceEquals(item, prisoner)))
            return;

        CrazyActionPerformed?.Invoke(prisoner);
    }

    private void ForwardLog(string message)
    {
        LogGenerated?.Invoke(message);
    }

}

