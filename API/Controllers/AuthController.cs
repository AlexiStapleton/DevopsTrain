using API.Dtos;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        try
        {
            var user = await authService.RegisterAsync(request, ct);
            return StatusCode(StatusCodes.Status201Created, user);
        }
        catch (EmailAlreadyUsedException)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Email deja utilise");
        }
    }
}