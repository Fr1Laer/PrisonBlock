namespace PrisonBlock.Core.Storage.EfCore;

// Esindab salvestatavat vangi põhiandmetega
public abstract class PrisonerEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Energy { get; set; }
    public RepositoryMembershipEntity? Membership { get; set; }
}

public sealed class RookieEntity : PrisonerEntity;
public sealed class PhilosopherEntity : PrisonerEntity;
public sealed class ChefEntity : PrisonerEntity;
public sealed class ArtistEntity : PrisonerEntity { public int Masterpieces { get; set; } }
public sealed class ChessMasterEntity : PrisonerEntity { public int Wins { get; set; } }
public sealed class GardenerEntity : PrisonerEntity { public int Plants { get; set; } }
public sealed class MusicianEntity : PrisonerEntity { public int Songs { get; set; } }
public sealed class GamblerEntity : PrisonerEntity { public int Cheats { get; set; } }

// Seob vangi tema tüübipõhise hoidlaga
public sealed class RepositoryMembershipEntity
{
    public Guid PrisonerId { get; set; }
    public string Group { get; set; } = "";
    public PrisonerEntity Prisoner { get; set; } = null!;
}
