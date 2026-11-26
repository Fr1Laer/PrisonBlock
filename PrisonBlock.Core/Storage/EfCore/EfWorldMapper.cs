namespace PrisonBlock.Core.Storage.EfCore;

// Teisendab salvestuslepingu EF Core'i entiteetideks
internal static class EfWorldMapper
{
    public static List<PrisonerEntity> ToEntities(WorldDto world)
    {
        WorldMapper.ValidateAndCreate(world);

        return world.Prisoners.Select(ToEntity).ToList();
    }

    public static WorldDto ToWorld(IEnumerable<PrisonerEntity> prisoners)
    {
        List<PrisonerDto> items = prisoners.Select(ToDto).ToList();
        var world = new WorldDto
        {
            Version = 1,
            SavedAt = DateTime.UtcNow,
            Prisoners = items
        };

        WorldMapper.ValidateAndCreate(world);
        return world;
    }

    private static PrisonerEntity ToEntity(PrisonerDto dto)
    {
        PrisonerEntity entity = dto.Type switch
        {
            nameof(Rookie) => new RookieEntity(),
            nameof(Philosopher) => new PhilosopherEntity(),
            nameof(Artist) => new ArtistEntity { Masterpieces = dto.Masterpieces },
            nameof(ChessMaster) => new ChessMasterEntity { Wins = dto.Wins },
            nameof(Chef) => new ChefEntity(),
            nameof(Gardener) => new GardenerEntity { Plants = dto.Plants },
            nameof(Musician) => new MusicianEntity { Songs = dto.Songs },
            nameof(Gambler) => new GamblerEntity { Cheats = dto.Cheats },
            _ => throw new InvalidDataException($"Tundmatu vangitüüp: {dto.Type}")
        };

        entity.Id = dto.Id;
        entity.Name = dto.Name;
        entity.Energy = dto.Energy;
        entity.Membership = new RepositoryMembershipEntity
        {
            PrisonerId = dto.Id,
            Group = dto.Group
        };

        return entity;
    }

    private static PrisonerDto ToDto(PrisonerEntity entity)
    {
        string type = entity switch
        {
            RookieEntity => nameof(Rookie),
            PhilosopherEntity => nameof(Philosopher),
            ArtistEntity => nameof(Artist),
            ChessMasterEntity => nameof(ChessMaster),
            ChefEntity => nameof(Chef),
            GardenerEntity => nameof(Gardener),
            MusicianEntity => nameof(Musician),
            GamblerEntity => nameof(Gambler),
            _ => throw new InvalidDataException("Tundmatu EF Core'i vangitüüp")
        };

        return new PrisonerDto
        {
            Id = entity.Id,
            Type = type,
            Group = entity.Membership?.Group ?? "",
            Name = entity.Name,
            Energy = entity.Energy,
            Masterpieces = (entity as ArtistEntity)?.Masterpieces ?? 0,
            Wins = (entity as ChessMasterEntity)?.Wins ?? 0,
            Plants = (entity as GardenerEntity)?.Plants ?? 0,
            Songs = (entity as MusicianEntity)?.Songs ?? 0,
            Cheats = (entity as GamblerEntity)?.Cheats ?? 0
        };
    }
}
