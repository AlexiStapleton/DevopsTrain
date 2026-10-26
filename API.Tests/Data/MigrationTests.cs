using API.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Data;
 
[Collection(PostgresCollection.Name)]
public class MigrationTests(PostgresFixture postgres)
{
    [Fact]
    public async Task Migrate_OnEmptyDatabase_AppliesInitialCreateMigration()
    {
        // Arrange : une base vide, propre à ce test
        await using var context = postgres.CreateContext(postgres.NewDatabaseConnectionString());
 
        // Act
        await context.Database.MigrateAsync();
 
        // Assert
        var applied = await context.Database.GetAppliedMigrationsAsync();
        Assert.Contains(applied, name => name.EndsWith("_InitialCreate"));
    }
}