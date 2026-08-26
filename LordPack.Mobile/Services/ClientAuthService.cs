using System.Net.Http.Json;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;

namespace LordPack.Mobile.Services;

public class ClientAuthService : IClientAuthService
{
    private readonly HttpClient _httpClient;

    public ClientAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsGuestAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/auth/guest", null);
            if (!response.IsSuccessStatusCode) return false;

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult == null) return false;

            await SaveSessionAsync(authResult);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode) return false;

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult == null) return false;

            await SaveSessionAsync(authResult);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            if (!response.IsSuccessStatusCode) return false;

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult == null) return false;

            await SaveSessionAsync(authResult);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<AuthResponseDto?> GetCurrentSessionAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token)) return null;

        var userId = await SecureStorage.Default.GetAsync("user_id") ?? string.Empty;
        var email = await SecureStorage.Default.GetAsync("user_email") ?? string.Empty;
        var fullName = await SecureStorage.Default.GetAsync("user_name") ?? string.Empty;
        var isGuestStr = await SecureStorage.Default.GetAsync("is_guest");
        bool.TryParse(isGuestStr, out var isGuest);

        return new AuthResponseDto
        {
            Token = token,
            UserId = userId,
            Email = email,
            FullName = fullName,
            IsGuest = isGuest
        };
    }

    public Task LogoutAsync()
    {
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("user_id");
        SecureStorage.Default.Remove("user_email");
        SecureStorage.Default.Remove("user_name");
        SecureStorage.Default.Remove("is_guest");
        return Task.CompletedTask;
    }

    private static async Task SaveSessionAsync(AuthResponseDto dto)
    {
        await SecureStorage.Default.SetAsync("auth_token", dto.Token);
        await SecureStorage.Default.SetAsync("user_id", dto.UserId);
        await SecureStorage.Default.SetAsync("user_email", dto.Email);
        await SecureStorage.Default.SetAsync("user_name", dto.FullName);
        await SecureStorage.Default.SetAsync("is_guest", dto.IsGuest.ToString());
    }
}