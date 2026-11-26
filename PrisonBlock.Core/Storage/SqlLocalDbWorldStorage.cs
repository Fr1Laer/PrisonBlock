using Microsoft.Data.SqlClient;

namespace PrisonBlock.Core.Storage;

public sealed class SqlLocalDbWorldStorage : IWorldStorage
{
    private const string DatabaseName = "PrisonBlock_Vol4";

    private readonly string _connectionString;

    public SqlLocalDbWorldStorage()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = @"(localdb)\MSSQLLocalDB",
            InitialCatalog = DatabaseName,
            IntegratedSecurity = true,
            TrustServerCertificate = true,
            ConnectTimeout = 30
        };

        _connectionString = builder.ConnectionString;
    }

    // Loob andmebaasi ja vajalikud tabelid esimesel kasutamisel
    private async Task EnsureDatabaseAsync(CancellationToken token)
    {
        var masterBuilder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        };

        await using (var connection = new SqlConnection(masterBuilder.ConnectionString))
        {
            await connection.OpenAsync(token);

            await using var command = connection.CreateCommand();

            command.CommandText = """
                IF DB_ID(@DatabaseName) IS NULL
                BEGIN
                    DECLARE @sql NVARCHAR(MAX) =
                        N'CREATE DATABASE ' + QUOTENAME(@DatabaseName);

                    EXEC sp_executesql @sql;
                END
                """;

            command.Parameters.AddWithValue("@DatabaseName", DatabaseName);

            await command.ExecuteNonQueryAsync(token);
        }

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(token);

            await using var command = connection.CreateCommand();

            command.CommandText = """
                IF OBJECT_ID('dbo.Prisoners', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Prisoners
                    (
                        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                        Type NVARCHAR(50) NOT NULL,
                        Name NVARCHAR(200) NOT NULL,
                        Energy INT NOT NULL,
                        Masterpieces INT NOT NULL DEFAULT 0,
                        Wins INT NOT NULL DEFAULT 0,
                        Plants INT NOT NULL DEFAULT 0,
                        Songs INT NOT NULL DEFAULT 0,
                        Cheats INT NOT NULL DEFAULT 0,
                        CONSTRAINT CK_Prisoners_Energy
                            CHECK (Energy BETWEEN 0 AND 100)
                    );
                END;

                IF OBJECT_ID('dbo.RepositoryMembership', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.RepositoryMembership
                    (
                        PrisonerId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                        RepositoryType NVARCHAR(50) NOT NULL,
                        CONSTRAINT FK_RepositoryMembership_Prisoners
                            FOREIGN KEY (PrisonerId)
                            REFERENCES dbo.Prisoners(Id)
                            ON DELETE CASCADE
                    );
                END;
                """;

            await command.ExecuteNonQueryAsync(token);
        }
    }

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        // Kontrollib andmeid enne andmebaasi muutmist
        WorldMapper.ValidateAndCreate(world);

        await EnsureDatabaseAsync(token);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(token);

        await using var transaction =
            (SqlTransaction)await connection.BeginTransactionAsync(token);

        try
        {
            await using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = """
                    DELETE FROM dbo.RepositoryMembership;
                    DELETE FROM dbo.Prisoners;
                    """;

                await deleteCommand.ExecuteNonQueryAsync(token);
            }

            foreach (PrisonerDto prisoner in world.Prisoners)
            {
                token.ThrowIfCancellationRequested();

                await using var command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = """
                    INSERT INTO dbo.Prisoners
                    (
                        Id, Type, Name, Energy,
                        Masterpieces, Wins, Plants, Songs, Cheats
                    )
                    VALUES
                    (
                        @Id, @Type, @Name, @Energy,
                        @Masterpieces, @Wins, @Plants, @Songs, @Cheats
                    );
                    """;

                command.Parameters.AddWithValue("@Id", prisoner.Id);
                command.Parameters.AddWithValue("@Type", prisoner.Type);
                command.Parameters.AddWithValue("@Name", prisoner.Name);
                command.Parameters.AddWithValue("@Energy", prisoner.Energy);
                command.Parameters.AddWithValue("@Masterpieces", prisoner.Masterpieces);
                command.Parameters.AddWithValue("@Wins", prisoner.Wins);
                command.Parameters.AddWithValue("@Plants", prisoner.Plants);
                command.Parameters.AddWithValue("@Songs", prisoner.Songs);
                command.Parameters.AddWithValue("@Cheats", prisoner.Cheats);

                await command.ExecuteNonQueryAsync(token);

                await using var membershipCommand = connection.CreateCommand();
                membershipCommand.Transaction = transaction;

                membershipCommand.CommandText = """
                    INSERT INTO dbo.RepositoryMembership
                        (PrisonerId, RepositoryType)
                    VALUES
                        (@PrisonerId, @RepositoryType);
                    """;

                membershipCommand.Parameters.AddWithValue(
                    "@PrisonerId", prisoner.Id);

                membershipCommand.Parameters.AddWithValue(
                    "@RepositoryType", prisoner.Group);

                await membershipCommand.ExecuteNonQueryAsync(token);
            }

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
        await EnsureDatabaseAsync(token);

        var world = new WorldDto
        {
            Version = 1,
            SavedAt = DateTime.UtcNow
        };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(token);

        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                p.Id,
                p.Type,
                p.Name,
                p.Energy,
                p.Masterpieces,
                p.Wins,
                p.Plants,
                p.Songs,
                p.Cheats,
                m.RepositoryType
            FROM dbo.Prisoners AS p
            INNER JOIN dbo.RepositoryMembership AS m
                ON p.Id = m.PrisonerId
            ORDER BY p.Name;
            """;

        await using var reader = await command.ExecuteReaderAsync(token);

        while (await reader.ReadAsync(token))
        {
            token.ThrowIfCancellationRequested();

            string type = reader.GetString(1);
            string repositoryType = reader.GetString(9);

            if (!string.Equals(type, repositoryType, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"Vangi {reader.GetGuid(0)} hoidla tüüp ei vasta tegelase tüübile");
            }

            world.Prisoners.Add(new PrisonerDto
            {
                Id = reader.GetGuid(0),
                Type = type,
                Group = repositoryType,
                Name = reader.GetString(2),
                Energy = reader.GetInt32(3),
                Masterpieces = reader.GetInt32(4),
                Wins = reader.GetInt32(5),
                Plants = reader.GetInt32(6),
                Songs = reader.GetInt32(7),
                Cheats = reader.GetInt32(8)
            });
        }

        // Kontrollib, et kõik loetud andmed on taastatavad
        WorldMapper.ValidateAndCreate(world);

        return world;
    }
}
