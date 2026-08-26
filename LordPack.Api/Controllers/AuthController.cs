using LordPack.Api.Interfaces;
using LordPack.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LordPack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthController(IJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("guest")]
    public ActionResult<AuthResponseDto> ContinueAsGuest()
    {
        var guestId = Guid.NewGuid().ToString();
        var response = _tokenGenerator.GenerateToken(guestId, "guest@lordpack.local", "Guest User", isGuest: true);
        return Ok(response);
    }

    [HttpPost("login")]
    public ActionResult<AuthResponseDto> Login([FromBody] LoginRequestDto request)
    {
        // Simple mock authentication for testing - replace with ASP.NET Identity or DB user check in production
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        var response = _tokenGenerator.GenerateToken(Guid.NewGuid().ToString(), request.Email, "User Account", isGuest: false);
        return Ok(response);
    }

    [HttpPost("register")]
    public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Invalid registration payload.");

        var response = _tokenGenerator.GenerateToken(Guid.NewGuid().ToString(), request.Email, request.FullName, isGuest: false);
        return Ok(response);
    }
}