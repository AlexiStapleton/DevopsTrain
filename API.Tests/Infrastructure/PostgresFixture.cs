using API.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace API.Tests.Infrastructure;

/// <summary>
/// Démarre UN conteneur PostgreSQL pour tous les tests de la collection.
/// Chaque test peut obtenir sa propre base (isolée) dans ce conteneur.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16").Build();

    public Task InitializeAsync() => _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();

    /// <summary>
    /// Chaîne de connexion vers une NOUVELLE base (nom unique) du conteneur.
    /// La base n'existe pas encore : MigrateAsync() la crée.
    /// </summary>
    public string NewDatabaseConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = $"test_{Guid.NewGuid():N}"
        };
        return builder.ConnectionString;
    }

    public AppDbContext CreateContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}

[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}