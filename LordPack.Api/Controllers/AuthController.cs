using LordPack.Api.Interfaces;
using LordPack.Api.Services;
using LordPack.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LordPack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(model);
        if (result == null) return BadRequest("Registration failed. Email may already be in use.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(model);
        if (result == null) return Unauthorized("Invalid email or password.");

        return Ok(result);
    }

    [HttpPost("guest")]
    public async Task<IActionResult> StartGuestSession()
    {
        var result = await _authService.CreateGuestSessionAsync();
        return Ok(result);
    }
}