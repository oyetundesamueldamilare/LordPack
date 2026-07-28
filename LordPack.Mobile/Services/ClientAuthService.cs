using System.Net.Http.Json;
using LordPack.Shared.DTOs;
using LordPack.Mobile.Interfaces;

namespace LordPack.Mobile.Services;


public class ClientAuthService : IClientAuthService
{
    private readonly HttpClient _httpClient;

    public ClientAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result == null || string.IsNullOrEmpty(result.Token)) return false;

        await SecureStorage.SetAsync("jwt_token", result.Token);
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result == null || string.IsNullOrEmpty(result.Token)) return false;

        await SecureStorage.SetAsync("jwt_token", result.Token);
        return true;
    }

    public async Task<bool> LoginAsGuestAsync()
    {
        var response = await _httpClient.PostAsync("api/auth/guest", null);
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (result == null || string.IsNullOrEmpty(result.Token)) return false;

        await SecureStorage.SetAsync("jwt_token", result.Token);
        return true;
    }

    public Task LogoutAsync()
    {
        SecureStorage.Remove("jwt_token");
        return Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await SecureStorage.GetAsync("jwt_token");
        return !string.IsNullOrEmpty(token);
    }
}