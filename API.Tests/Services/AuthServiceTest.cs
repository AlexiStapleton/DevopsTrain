using API.Data;
using API.Dtos;
using API.Exceptions;
using API.Models;
using API.Services;
using API.Tests.Fakes;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace API.Tests.Services;

[TestSubject(typeof(AuthService))]
public class AuthServiceTest
{
    private readonly FakeUserRepository _users = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly AuthService _sut;
 
    public AuthServiceTest()
    {
        // xUnit cree une nouvelle instance de la classe pour CHAQUE test :
        // le faux repository repart donc vide a chaque fois.
        _sut = new AuthService(_users, _hasher);
    }
 
    private static RegisterRequest ValidRequest(string email = "alice@example.com") => new()
    {
        FirstName = "Alice",
        LastName = "Martin",
        Email = email,
        Password = "CorrectHorse-Battery9",
        PasswordConfirm = "CorrectHorse-Battery9"
    };
 
    [Fact]
    public async Task Register_WithValidRequest_StoresUserWithHashedPassword()
    {
        var request = ValidRequest();
 
        var response = await _sut.RegisterAsync(request, CancellationToken.None);
 
        var stored = Assert.Single(_users.Users);
        Assert.Equal("alice@example.com", stored.Email);
        Assert.NotEqual(request.Password, stored.PasswordHash);
        var verification = _hasher.VerifyHashedPassword(stored, stored.PasswordHash, request.Password);
        Assert.Equal(PasswordVerificationResult.Success, verification);
        Assert.Equal("alice@example.com", response.Email);
    }
 
    [Fact]
    public async Task Register_NormalizesEmail()
    {
        await _sut.RegisterAsync(ValidRequest("  Alice@Example.COM "), CancellationToken.None);
 
        var stored = Assert.Single(_users.Users);
        Assert.Equal("alice@example.com", stored.Email);
    }
 
    [Fact]
    public async Task Register_WithExistingEmail_ThrowsAndDoesNotAddSecondUser()
    {
        await _sut.RegisterAsync(ValidRequest(), CancellationToken.None);
 
        await Assert.ThrowsAsync<EmailAlreadyUsedException>(() =>
            _sut.RegisterAsync(ValidRequest(), CancellationToken.None));
 
        Assert.Single(_users.Users);
    }
 
    [Fact]
    public async Task Register_WithExistingEmailInDifferentCase_Throws()
    {
        await _sut.RegisterAsync(ValidRequest("alice@example.com"), CancellationToken.None);
 
        await Assert.ThrowsAsync<EmailAlreadyUsedException>(() =>
            _sut.RegisterAsync(ValidRequest("ALICE@example.com"), CancellationToken.None));
    }
 
    [Fact]
    public async Task Register_SetsUtcTimestamps()
    {
        var response = await _sut.RegisterAsync(ValidRequest(), CancellationToken.None);
 
        var stored = Assert.Single(_users.Users);
        Assert.Equal(DateTimeKind.Utc, stored.CreatedAt.Kind);
        Assert.Equal(stored.CreatedAt, stored.UpdatedAt);
        Assert.Equal(stored.CreatedAt, response.CreatedAt);
    }
}