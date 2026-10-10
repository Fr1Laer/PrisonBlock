using System.Data;
using System.Reflection.Metadata;

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
}
