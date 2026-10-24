using API.Models;
using API.Repositories;

namespace API.Tests.Fakes;

/// <summary>
/// Faux repository en mémoire pour les tests unitaires des services.
/// Il garde un état (la liste), donc les tests vérifient des RÉSULTATS, pas des appels.
/// </summary>
public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
        Task.FromResult(Users.Any(u => u.Email == email));

    public Task AddAsync(User user, CancellationToken ct)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }
}