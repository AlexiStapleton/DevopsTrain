using System.IdentityModel.Tokens.Jwt;
using API.Models;
using API.Services;
using API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace API.Tests.Auth;

public class TokenServiceTests
{
    // Valeurs de test uniquement
    private static readonly JwtOptions Jwt = new()
    {
        Key = "unit-test-signing-key-0123456789-abcdefghij", // 43 caracteres (>= 32)
        Issuer = "test-issuer",
        Audience = "test-audience"
    };

    // Horloge figee : on controle l'heure, aucun Thread.Sleep necessaire
    private readonly FakeTimeProvider _clock =
        new(new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero));

    private readonly TokenService _sut;

    public TokenServiceTests()
    {
        _sut = new TokenService(Options.Create(Jwt), _clock);
    }

    private static User NewUser() => new()
    {
        Id = 42,
        FirstName = "Alice",
        LastName = "Martin",
        Email = "alice@example.com",
        PasswordHash = "hash"
    };

    [Fact]
    public void CreateToken_ExpiresThirtyMinutesAfterNow()
    {
        var token = _sut.CreateToken(NewUser());

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(_clock.GetUtcNow().UtcDateTime.AddMinutes(30), parsed.ValidTo);
    }

    [Fact]
    public void METHOD()
    {
        
    }
}