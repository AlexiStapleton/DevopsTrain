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
    private const string Email = "alice@example.com";
    private const string Password = "CorrectHorse-Battery9";
    
    private readonly FakeUserRepository _users = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly FakeTokenService _tokens = new();
    private readonly AuthService _sut;

    public AuthServiceTest()
    {
        // xUnit creates a new instance of this class for EACH test:
        // the fake repository and the fake token service start fresh every time.
        _sut = new AuthService(_users, _hasher, _tokens);
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

        var result = await _sut.RegisterAsync(request, CancellationToken.None);

        Assert.Equal(LoginStatus.Success, result.Status);

        var stored = Assert.Single(_users.Users);
        Assert.Equal("alice@example.com", stored.Email);
        Assert.NotEqual(request.Password, stored.PasswordHash);
        var verification = _hasher.VerifyHashedPassword(stored, stored.PasswordHash, request.Password);
        Assert.Equal(PasswordVerificationResult.Success, verification);
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsTokenForTheNewUser()
    {
        var result = await _sut.RegisterAsync(ValidRequest(), CancellationToken.None);

        Assert.Equal(LoginStatus.Success, result.Status);
        Assert.Equal("fake-token-for-alice@example.com", result.Token);
        Assert.Equal(1, _tokens.CallCount);
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
    public async Task Register_WithExistingEmail_DoesNotIssueASecondToken()
    {
        await _sut.RegisterAsync(ValidRequest(), CancellationToken.None);

        await Assert.ThrowsAsync<EmailAlreadyUsedException>(() =>
            _sut.RegisterAsync(ValidRequest(), CancellationToken.None));

        Assert.Equal(1, _tokens.CallCount); // only the first registration got a token
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
        await _sut.RegisterAsync(ValidRequest(), CancellationToken.None);

        var stored = Assert.Single(_users.Users);
        Assert.Equal(DateTimeKind.Utc, stored.CreatedAt.Kind);
        Assert.Equal(stored.CreatedAt, stored.UpdatedAt);
    }

    /// <summary>Registers Alice through the real service, so the stored hash is a real one.</summary>
    private async Task RegisterAliceAsync()
    {
        await _sut.RegisterAsync(new RegisterRequest
        {
            FirstName = "Alice",
            LastName = "Martin",
            Email = Email,
            Password = Password,
            PasswordConfirm = Password
        }, CancellationToken.None);
    }
 
    private static LoginRequest Login(string email, string password) =>
        new() { Email = email, Password = password };
 
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
    {
        await RegisterAliceAsync();
 
        var result = await _sut.LoginAsync(Login(Email, Password), CancellationToken.None);
 
        Assert.Equal(LoginStatus.Success, result.Status);
        Assert.Equal("fake-token-for-alice@example.com", result.Token);
    }
 
    [Fact]
    public async Task Login_WithEmailInDifferentCaseAndSpaces_Succeeds()
    {
        await RegisterAliceAsync();
 
        var result = await _sut.LoginAsync(Login("  ALICE@Example.COM ", Password), CancellationToken.None);
 
        Assert.Equal(LoginStatus.Success, result.Status);
        Assert.NotNull(result.Token);
    }
 
    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsInvalidCredentialsWithoutToken()
    {
        await RegisterAliceAsync();
 
        var result = await _sut.LoginAsync(Login("bob@example.com", Password), CancellationToken.None);
 
        Assert.Equal(LoginStatus.InvalidCredentials, result.Status);
        Assert.Null(result.Token);
    }
 
    [Fact]
    public async Task Login_WithWrongPassword_ReturnsInvalidCredentialsWithoutToken()
    {
        await RegisterAliceAsync();
 
        var result = await _sut.LoginAsync(Login(Email, "Wrong-Password-12345"), CancellationToken.None);
 
        Assert.Equal(LoginStatus.InvalidCredentials, result.Status);
        Assert.Null(result.Token);
    }
 
    [Fact]
    public async Task Login_PasswordIsCaseSensitive()
    {
        await RegisterAliceAsync();
 
        var result = await _sut.LoginAsync(Login(Email, Password.ToLowerInvariant()), CancellationToken.None);
 
        Assert.Equal(LoginStatus.InvalidCredentials, result.Status);
    }
 
    [Fact]
    public async Task Login_UnknownEmailAndWrongPassword_ReturnExactlyTheSameResult()
    {
        await RegisterAliceAsync();
 
        var unknownEmail = await _sut.LoginAsync(Login("bob@example.com", Password), CancellationToken.None);
        var wrongPassword = await _sut.LoginAsync(Login(Email, "Wrong-Password-12345"), CancellationToken.None);
 
        // LoginResult is a record: Assert.Equal compares its values.
        // Same result for both failures => the API cannot reveal which emails exist.
        Assert.Equal(unknownEmail, wrongPassword);
    }
 
    [Fact]
    public async Task Login_WithInvalidCredentials_DoesNotIssueAnyToken()
    {
        await RegisterAliceAsync();
        var tokensAfterRegistration = _tokens.CallCount;
 
        await _sut.LoginAsync(Login(Email, "Wrong-Password-12345"), CancellationToken.None);
        await _sut.LoginAsync(Login("bob@example.com", Password), CancellationToken.None);
 
        Assert.Equal(tokensAfterRegistration, _tokens.CallCount);
    }
}