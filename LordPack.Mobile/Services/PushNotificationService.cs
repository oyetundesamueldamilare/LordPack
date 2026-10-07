using System.Net.Http.Json;
using LordPack.Mobile.Interfaces;

namespace LordPack.Mobile.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly HttpClient _httpClient;

    public PushNotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task InitializeAsync()
    {
        // Platform notification permission request & channel configuration
        return Task.CompletedTask;
    }

    public async Task<string?> GetDeviceTokenAsync()
    {
        // Retrieve local token or platform-registered push token
        return await SecureStorage.Default.GetAsync("push_device_token");
    }

    public async Task RegisterDeviceTokenWithApiAsync()
    {
        var token = await GetDeviceTokenAsync();
        if (string.IsNullOrEmpty(token)) return;

        try
        {
            var payload = new { DeviceToken = token, Platform = DeviceInfo.Platform.ToString() };
            await _httpClient.PostAsJsonAsync("api/notifications/register-device", payload);
        }
        catch
        {
            // Silent retry on next session initialization
        }
    }
}