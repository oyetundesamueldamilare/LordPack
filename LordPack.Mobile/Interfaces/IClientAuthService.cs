using LordPack.Shared.DTOs;

namespace LordPack.Mobile.Interfaces;

public interface IClientAuthService
{
    Task<bool> LoginAsync(LoginRequestDto dto);
    Task<bool> RegisterAsync(RegisterRequestDto dto);
    Task<bool> LoginAsGuestAsync();
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<AuthResponseDto?> GetCurrentSessionAsync();
}