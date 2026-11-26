using Microsoft.EntityFrameworkCore;

namespace PrisonBlock.Core.Storage.EfCore;

// Seadistab EF Core'i TPH andmemudeli
public sealed class PrisonBlockDbContext : DbContext
{
    public DbSet<PrisonerEntity> Prisoners => Set<PrisonerEntity>();
    public DbSet<RepositoryMembershipEntity> RepositoryMemberships => Set<RepositoryMembershipEntity>();

    public PrisonBlockDbContext(DbContextOptions<PrisonBlockDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrisonerEntity>(entity =>
        {
            entity.ToTable("Prisoners");
            entity.HasKey(prisoner => prisoner.Id);
            entity.Property(prisoner => prisoner.Name).HasMaxLength(200).IsRequired();
            entity.Property(prisoner => prisoner.Energy).IsRequired();
            entity.HasDiscriminator<string>("PrisonerType")
                .HasValue<RookieEntity>(nameof(Rookie))
                .HasValue<PhilosopherEntity>(nameof(Philosopher))
                .HasValue<ArtistEntity>(nameof(Artist))
                .HasValue<ChessMasterEntity>(nameof(ChessMaster))
                .HasValue<ChefEntity>(nameof(Chef))
                .HasValue<GardenerEntity>(nameof(Gardener))
                .HasValue<MusicianEntity>(nameof(Musician))
                .HasValue<GamblerEntity>(nameof(Gambler));
        });

        modelBuilder.Entity<RepositoryMembershipEntity>(entity =>
        {
            entity.ToTable("RepositoryMemberships");
            entity.HasKey(membership => membership.PrisonerId);
            entity.Property(membership => membership.Group).HasMaxLength(50).IsRequired();
            entity.HasOne(membership => membership.Prisoner)
                .WithOne(prisoner => prisoner.Membership)
                .HasForeignKey<RepositoryMembershipEntity>(membership => membership.PrisonerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
