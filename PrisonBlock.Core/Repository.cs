namespace PrisonBlock.Core;

public sealed class Repository<T> :
    IRepository<T>,
    IPrisonerEventSource where T : Prisoner
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

        Added?.Invoke(item);

        item.ReactionLogged += ForwardLog;
        item.Subscribe(this);

        LogGenerated?.Invoke($"Lisati {item.Name} ({typeof(T).Name})");
    }

    // Taastab salvestatud objekti ilma reaktsioone käivitamata
    internal void Restore(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.Any(existing => existing.Id == item.Id))
            throw new InvalidOperationException("Duplikaatne vangi ID");

        _items.Add(item);
        item.ReactionLogged += ForwardLog;
        item.Subscribe(this);
    }

    internal void ReplaceAll(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        List<T> newItems = items.ToList();

        if (newItems.Any(item => item is null))
            throw new ArgumentException("Hoidla ei saa sisaldada tühje elemente", nameof(items));

        if (newItems.Distinct().Count() != newItems.Count)
            throw new InvalidOperationException("Hoidlas on korduvad objektid");

        foreach (T oldItem in _items)
        {
            oldItem.Unsubscribe(this);
            oldItem.ReactionLogged -= ForwardLog;
        }

        _items.Clear();

        foreach (T newItem in newItems)
        {
            _items.Add(newItem);
            newItem.ReactionLogged += ForwardLog;
            newItem.Subscribe(this);
        }
    }

    public bool Remove(T item)
    {
        if (item is null || !_items.Contains(item))
            return false;

        item.Unsubscribe(this);
        item.ReactionLogged -= ForwardLog;
        _items.Remove(item);

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