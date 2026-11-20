namespace PrisonBlock.Core.Storage;

public static class WorldMapper
{
    // Teisendab vangla maailma salvestatavaks DTO-ks
    public static WorldDto ToDto(IEnumerable<Prisoner> prisoners)
    {
        ArgumentNullException.ThrowIfNull(prisoners);

        return new WorldDto
        {
            Version = 1,
            SavedAt = DateTime.UtcNow,
            Prisoners = prisoners.Select(ToDto).ToList()
        };
    }

    private static PrisonerDto ToDto(Prisoner prisoner)
    {
        var dto = new PrisonerDto
        {
            Id = prisoner.Id,
            Type = prisoner.GetType().Name,
            Name = prisoner.Name,
            Energy = prisoner.Energy
        };

        switch (prisoner)
        {
            case Artist artist:
                dto.Masterpieces = artist.Masterpieces;
                break;
            case ChessMaster chessMaster:
                dto.Wins = chessMaster.Wins;
                break;
            case Gardener gardener:
                dto.Plants = gardener.Plants;
                break;
            case Musician musician:
                dto.Songs = musician.Songs;
                break;
            case Gambler gambler:
                dto.Cheats = gambler.Cheats;
                break;
        }

        return dto;
    }

    // Taastab konkreetsed vangitüübid DTO-andmete põhjal
    public static Prisoner FromDto(PrisonerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Id == Guid.Empty)
            throw new InvalidDataException("Vangi ID puudub");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new InvalidDataException("Vangi nimi puudub");

        if (dto.Energy is < 0 or > 100)
            throw new InvalidDataException("Vangi energia ei ole lubatud vahemikus");

        return dto.Type switch
        {
            nameof(Rookie) =>
                new Rookie(dto.Id, dto.Name, dto.Energy),

            nameof(Philosopher) =>
                new Philosopher(dto.Id, dto.Name, dto.Energy),

            nameof(Artist) =>
                new Artist(dto.Id, dto.Name, dto.Energy, dto.Masterpieces),

            nameof(ChessMaster) =>
                new ChessMaster(dto.Id, dto.Name, dto.Energy, dto.Wins),

            nameof(Chef) =>
                new Chef(dto.Id, dto.Name, dto.Energy),

            nameof(Gardener) =>
                new Gardener(dto.Id, dto.Name, dto.Energy, dto.Plants),

            nameof(Musician) =>
                new Musician(dto.Id, dto.Name, dto.Energy, dto.Songs),

            nameof(Gambler) =>
                new Gambler(dto.Id, dto.Name, dto.Energy, dto.Cheats),

            _ => throw new InvalidDataException(
                $"Tundmatu vangitüüp: {dto.Type}")
        };
    }

    // Kontrollib maailma enne selle rakendusse laadimist
    public static List<Prisoner> ValidateAndCreate(WorldDto world)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (world.Version != 1)
            throw new InvalidDataException("Maailma versioon ei ole toetatud");

        if (world.Prisoners is null)
            throw new InvalidDataException("Vangide loend puudub");

        var prisoners = world.Prisoners
            .Select(FromDto)
            .ToList();

        if (prisoners.Select(p => p.Id).Distinct().Count() != prisoners.Count)
            throw new InvalidDataException("Maailmas on korduvad vangi ID-d");

        return prisoners;
    }
}