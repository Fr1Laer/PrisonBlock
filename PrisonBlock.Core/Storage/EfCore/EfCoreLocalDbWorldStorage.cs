using Microsoft.EntityFrameworkCore;

namespace PrisonBlock.Core.Storage.EfCore;

// Salvestab maailma EF Core'i abil LocalDB-sse
public sealed class EfCoreLocalDbWorldStorage : IWorldStorage
{
    private readonly IDbContextFactory<PrisonBlockDbContext> _contextFactory;

    public EfCoreLocalDbWorldStorage(IDbContextFactory<PrisonBlockDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        List<PrisonerEntity> prisoners = EfWorldMapper.ToEntities(world);
        await using PrisonBlockDbContext context = await _contextFactory.CreateDbContextAsync(token);

        // Rakendab skeemi migratsioonid enne salvestamist
        await context.Database.MigrateAsync(token);

        await using var transaction = await context.Database.BeginTransactionAsync(token);

        try
        {
            context.RepositoryMemberships.RemoveRange(context.RepositoryMemberships);
            context.Prisoners.RemoveRange(context.Prisoners);

            await context.SaveChangesAsync(token);
            await context.Prisoners.AddRangeAsync(prisoners, token);
            await context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default)
    {
        await using PrisonBlockDbContext context = await _contextFactory.CreateDbContextAsync(token);
        // Rakendab skeemi migratsioonid enne laadimist
        await context.Database.MigrateAsync(token);

        List<PrisonerEntity> prisoners = await context.Prisoners
            .Include(prisoner => prisoner.Membership)
            .OrderBy(prisoner => prisoner.Name)
            .ToListAsync(token);

        return EfWorldMapper.ToWorld(prisoners);
    }
}
