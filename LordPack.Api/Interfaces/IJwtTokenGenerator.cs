using LordPack.Shared.DTOs;

namespace LordPack.Api.Interfaces
{
    public interface IJwtTokenGenerator
    {
        AuthResponseDto GenerateToken(string userId, string email, string fullName, bool isGuest = false);
    }
}
