using LordPack.Mobile.Interfaces;

namespace LordPack.Mobile;

public partial class App : Application
{
    private readonly IPushNotificationService _pushNotificationService;

    public App(IPushNotificationService pushNotificationService)
    {
        InitializeComponent();
        _pushNotificationService = pushNotificationService;

        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _pushNotificationService.InitializeAsync();
        await _pushNotificationService.RegisterDeviceTokenWithApiAsync();
    }
}