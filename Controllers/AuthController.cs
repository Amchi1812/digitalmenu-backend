using DigitalMenu.DTOs;
using DigitalMenu.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new { message = "Neispravan email ili lozinka." });
        }

        return Ok(result);
    }
}