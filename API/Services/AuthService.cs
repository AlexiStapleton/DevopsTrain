using API.Dtos;
using API.Exceptions;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace API.Services;


public class AuthService(IUserRepository users, IPasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
 
        if (await users.ExistsByEmailAsync(email, ct))
            throw new EmailAlreadyUsedException();
 
        var now = DateTime.UtcNow;
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = string.Empty,
            CreatedAt = now,
            UpdatedAt = now
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
 
        await users.AddAsync(user, ct);
 
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}