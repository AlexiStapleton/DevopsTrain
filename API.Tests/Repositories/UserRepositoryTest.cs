using API.Exceptions;
using API.Models;
using API.Repositories;
using API.Tests.Infrastructure;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Repositories;


[Collection(PostgresCollection.Name)]
public class UserRepositoryTest(PostgresFixture postgres)
{
    private async Task<string> CreateMigratedDatabaseAsync()
    {
        var connectionString = postgres.NewDatabaseConnectionString();
        await using var context = postgres.CreateContext(connectionString);
        await context.Database.MigrateAsync();
        return connectionString;
    }
 
    private static User NewUser(string email = "alice@example.com") => new()
    {
        FirstName = "Alice",
        LastName = "Martin",
        Email = email,
        PasswordHash = "hash",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
 
    [Fact]
    public async Task AddAsync_ThenFindByEmail_ReturnsStoredUser()
    {
        var connectionString = await CreateMigratedDatabaseAsync();
 
        await using (var writeContext = postgres.CreateContext(connectionString))
        {
            await new UserRepository(writeContext).AddAsync(NewUser(), CancellationToken.None);
        }
 
        // Nouveau contexte pour la lecture : on lit vraiment la base, pas le cache EF
        await using var readContext = postgres.CreateContext(connectionString);
        var found = await new UserRepository(readContext)
            .FindByEmailAsync("alice@example.com", CancellationToken.None);
 
        Assert.NotNull(found);
        Assert.True(found.Id > 0);
        Assert.Equal("Alice", found.FirstName);
    }
 
    [Fact]
    public async Task AddAsync_WithDuplicateEmail_ThrowsEmailAlreadyUsed()
    {
        var connectionString = await CreateMigratedDatabaseAsync();
 
        await using (var first = postgres.CreateContext(connectionString))
        {
            await new UserRepository(first).AddAsync(NewUser(), CancellationToken.None);
        }
 
        await using var second = postgres.CreateContext(connectionString);
        await Assert.ThrowsAsync<EmailAlreadyUsedException>(() =>
            new UserRepository(second).AddAsync(NewUser(), CancellationToken.None));
    }
 
    [Fact]
    public async Task ExistsByEmailAsync_ReflectsStoredUsers()
    {
        var connectionString = await CreateMigratedDatabaseAsync();
 
        await using var context = postgres.CreateContext(connectionString);
        var repository = new UserRepository(context);
 
        Assert.False(await repository.ExistsByEmailAsync("alice@example.com", CancellationToken.None));
 
        await repository.AddAsync(NewUser(), CancellationToken.None);
 
        Assert.True(await repository.ExistsByEmailAsync("alice@example.com", CancellationToken.None));
    }
}