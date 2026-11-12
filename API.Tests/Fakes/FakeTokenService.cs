using API.Models;
using API.Services;

namespace API.Tests.Fakes;

/// <summary>
/// Fake token service for unit tests: no key, no configuration, no clock.
/// </summary>
public class FakeTokenService : ITokenService
{
    public int CallCount { get; private set; }

    public string CreateToken(User user)
    {
        CallCount++;
        return $"fake-token-for-{user.Email}";
    }
}