using System.Net;
using System.Net.Http.Json;
using API.Data;
using API.Dtos;
using API.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Auth;

[Collection(PostgresCollection.Name)]
public class RegisterEndpointTest(PostgresFixture postgres) : IAsyncLifetime
{
    private ApiFactory _factory = null!;
    private HttpClient _client = null!;

    public Task InitializeAsync()
    {
        // Une base neuve par test : le demarrage de l'API applique les migrations
        _factory = new ApiFactory(postgres.NewDatabaseConnectionString());
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
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
    public async Task Register_WithValidRequest_Returns201WithoutSecrets()
    {
        var request = ValidRequest();

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(request.Password, body);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(user);
        Assert.True(user.Id > 0);
        Assert.Equal("alice@example.com", user.Email);
    }

    [Fact]
    public async Task Register_WithValidRequest_PersistsUserWithHashedPassword()
    {
        var request = ValidRequest();

        await _client.PostAsJsonAsync("/api/auth/register", request);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var stored = await db.Users.SingleAsync();
        Assert.NotEqual(request.Password, stored.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(stored.PasswordHash));
    }

    [Fact]
    public async Task Register_WithExistingEmail_Returns409()
    {
        await _client.PostAsJsonAsync("/api/auth/register", ValidRequest());

        var response = await _client.PostAsJsonAsync("/api/auth/register", ValidRequest("ALICE@example.com"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("not-an-email", "CorrectHorse-Battery9", "CorrectHorse-Battery9")]
    [InlineData("alice@example.com", "short", "short")]
    [InlineData("alice@example.com", "CorrectHorse-Battery9", "Different-Password1")]
    public async Task Register_WithInvalidInput_Returns400(string email, string password, string confirm)
    {
        var request = ValidRequest(email);
        request.Password = password;
        request.PasswordConfirm = confirm;

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMissingFields_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}