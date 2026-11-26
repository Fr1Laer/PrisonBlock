using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PrisonBlock.Core.Storage.EfCore;

// Loob migratsioonide jaoks LocalDB konteksti
public sealed class PrisonBlockDbContextFactory : IDesignTimeDbContextFactory<PrisonBlockDbContext>
{
    public PrisonBlockDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<PrisonBlockDbContext> options =
            new DbContextOptionsBuilder<PrisonBlockDbContext>()
                .UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=PrisonBlock_Vol5;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;

        return new PrisonBlockDbContext(options);
    }
}
