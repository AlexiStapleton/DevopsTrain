using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos;
using API.Exceptions;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;


public class AuthService(IUserRepository users, IPasswordHasher<User> passwordHasher, ITokenService tokens) : IAuthService
{
    public async Task<LoginResult> RegisterAsync(RegisterRequest request, CancellationToken ct)
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
        
        return new LoginResult(LoginStatus.Success, tokens.CreateToken(user));
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.FindByEmailAsync(email, ct);
 
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }
 
        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
 
        // SuccessRehashNeeded = mot de passe correct, hash a regenerer plus tard : on l'accepte
        return new LoginResult(LoginStatus.Success, tokens.CreateToken(user));
    }
}
