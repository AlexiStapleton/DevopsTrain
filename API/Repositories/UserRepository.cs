using API.Data;
using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
 
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
        context.Users.AnyAsync(u => u.Email == email, ct);
 
    public async Task AddAsync(User user, CancellationToken ct)
    {
        context.Users.Add(user);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Deux inscriptions simultanees avec le meme email : l'index unique a protege la base.
            context.Entry(user).State = EntityState.Detached;
            throw new EmailAlreadyUsedException();
        }
    }
}