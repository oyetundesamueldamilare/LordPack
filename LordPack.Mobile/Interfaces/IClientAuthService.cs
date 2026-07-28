using LordPack.Shared.DTOs;

namespace LordPack.Mobile.Interfaces;

public interface IClientAuthService
{
    Task<bool> LoginAsync(LoginDto dto);
    Task<bool> RegisterAsync(RegisterDto dto);
    Task<bool> LoginAsGuestAsync();
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}