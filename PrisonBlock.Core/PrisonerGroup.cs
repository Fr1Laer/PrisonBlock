namespace PrisonBlock.Core;

// Kirjeldab vangi tüübipõhist hoidlarühma
public sealed record PrisonerGroup
{
    private PrisonerGroup(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static PrisonerGroup FromPrisoner(Prisoner prisoner)
    {
        ArgumentNullException.ThrowIfNull(prisoner);
        return FromType(prisoner.GetType().Name);
    }

    public static PrisonerGroup FromType(string type)
    {
        if (!KnownTypes.Contains(type, StringComparer.Ordinal))
            throw new ArgumentException("Tundmatu vangirühm", nameof(type));

        return new PrisonerGroup(type);
    }

    private static readonly string[] KnownTypes =
    [
        nameof(Rookie), nameof(Philosopher), nameof(Artist),
        nameof(ChessMaster), nameof(Chef), nameof(Gardener),
        nameof(Musician), nameof(Gambler)
    ];
}
