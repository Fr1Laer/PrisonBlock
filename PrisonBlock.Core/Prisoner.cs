using System.Data;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace PrisonBlock.Core;

public abstract class Prisoner
{
    private readonly string _name;

    private int _energy;

    protected Prisoner(string name, int energy = 70)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nimi on kohustuslik");

        _name = name.Trim();

        _energy = Math.Clamp(energy, 0, 100);
    }

    public string Name => _name;

    public int Energy => _energy;

    protected bool TrySpendEnergy(int amount)
    {
        if (amount < 0 || _energy < amount)
            return false;

        _energy -= amount;

        return true;
    }

    protected void RestoreEnergy(int amount)
    {
        if (amount > 0)
            _energy = Math.Clamp(_energy + amount, 0, 100);
    }

    public virtual string GetDetails() => $"{Name} | {GetType().Name} | Energia: {Energy}/100";
    public abstract string CrazyAction();

    public event Action<string>? ReactionLogged; 

    internal void Subscribe(IPrisonerEventSource source)
    {
        source.Added += OnAdded;
        source.Removing += OnRemoving;
        source.CrazyActionPerformed += OnCrazyActionPerformed;
    }

    internal void Unsubscribe(IPrisonerEventSource source)
    {
        source.Added -= OnAdded;
        source.Removing -= OnRemoving;
        source.CrazyActionPerformed -= OnCrazyActionPerformed;
    }

    private void OnAdded(Prisoner prisoner)
    {
        if (ReferenceEquals(this, prisoner))
            return;

        RestoreEnergy(2);
        ReactionLogged?.Invoke($"{Name} tervitab uut tegelast {prisoner.Name}. Energia +2");
    }

    private void OnRemoving(Prisoner prisoner)
    {
        if (ReferenceEquals(this, prisoner))
            return;

        ReactionLogged?.Invoke($"{Name} jätab tegelasega {prisoner.Name} hüvasti");
    }

    private void OnCrazyActionPerformed(Prisoner prisoner)
    {
        if (ReferenceEquals(this, prisoner))
            return;

        ReactionLogged?.Invoke($"{Name} reageerib tegelase {prisoner.Name} pöörasele tegevusele");
    }
}
