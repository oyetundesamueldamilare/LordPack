using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using LordPack.Shared.DTOs;
using LordPack.Shared.Models;
using LordPack.Api.Interfaces;

namespace LordPack.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null) return null; // Email already in use

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsGuest = false
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return null;

        return GenerateJwtToken(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsGuest) return null;

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isValidPassword) return null;

        return GenerateJwtToken(user);
    }

    public async Task<AuthResponseDto> CreateGuestSessionAsync()
    {
        // Generate an ephemeral guest account
        var guestUser = new AppUser
        {
            UserName = $"guest_{Guid.NewGuid():N}@lordpack.local",
            Email = $"guest_{Guid.NewGuid():N}@lordpack.local",
            FullName = "Guest Listener",
            IsGuest = true
        };

        await _userManager.CreateAsync(guestUser);
        return GenerateJwtToken(guestUser);
    }

    private AuthResponseDto GenerateJwtToken(AppUser user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "SuperSecretLordPackKey2026!KeyMustBe32BytesMinimum");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("FullName", user.FullName),
            new Claim("IsGuest", user.IsGuest.ToString().ToLower())
        };

        var expiration = user.IsGuest ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(30);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = jwtSettings["Issuer"] ?? "LordPack.Api",
            Audience = jwtSettings["Audience"] ?? "LordPack.Mobile",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            IsGuest = user.IsGuest,
            Expiration = expiration
        };
    }
}