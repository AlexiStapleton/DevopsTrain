using API.Dtos;

namespace API.Services;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
}