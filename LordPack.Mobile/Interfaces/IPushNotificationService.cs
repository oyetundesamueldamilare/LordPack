namespace LordPack.Mobile.Interfaces;

public interface IPushNotificationService
{
    Task InitializeAsync();
    Task<string?> GetDeviceTokenAsync();
    Task RegisterDeviceTokenWithApiAsync();
}