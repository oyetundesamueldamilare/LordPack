using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LordPack.Api.Interfaces;
using LordPack.Shared.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace LordPack.Api.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    public AuthResponseDto GenerateToken(string userId, string email, string fullName, bool isGuest = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"] ?? "SuperSecretKey_LordPack_2026_Key_Must_Be_Long!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(7);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("fullName", fullName),
            new Claim("isGuest", isGuest.ToString().ToLower())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "LordPackApi",
            audience: _config["Jwt:Audience"] ?? "LordPackMobile",
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = userId,
            Email = email,
            FullName = fullName,
            IsGuest = isGuest,
            Expiration = expiration
        };
    }
}