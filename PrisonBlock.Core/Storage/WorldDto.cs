namespace PrisonBlock.Core.Storage;

public sealed class WorldDto
{
    public int Version { get; set; } = 1;

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    public List<PrisonerDto> Prisoners { get; set; } = new();
}

public sealed class PrisonerDto
{
    public Guid Id { get; set; }

    public string Type { get; set; } = "";

    public string Name { get; set; } = "";

    public int Energy { get; set; }

    public int Masterpieces { get; set; }

    public int Wins { get; set; }

    public int Plants { get; set; }

    public int Songs { get; set; }

    public int Cheats { get; set; }
}