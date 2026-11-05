using API.Dtos;

namespace API.Services;

public interface IAuthService
{
    Task<LoginResult> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct);
}