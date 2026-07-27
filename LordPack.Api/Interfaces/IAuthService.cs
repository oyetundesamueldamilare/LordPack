using LordPack.Shared.DTOs;

namespace LordPack.Api.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto model);
        Task<AuthResponseDto> CreateGuestSessionAsync();
    }
}
