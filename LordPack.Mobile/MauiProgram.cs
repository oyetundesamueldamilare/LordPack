using LordPack.Mobile.Handlers;
using LordPack.Mobile.Services;
using LordPack.Mobile.ViewModels;
using LordPack.Mobile.Views; // <--- Ensure this using directive is present
using LordPack.Mobile.Interfaces;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace LordPack.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // 1. Configure Base HTTP Client URL
        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7147/"
            : "https://localhost:7147/";

        builder.Services.AddTransient<JwtAuthHandler>();

        builder.Services.AddHttpClient("LordPackApi", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .AddHttpMessageHandler<JwtAuthHandler>();

        builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("LordPackApi"));

        // 2. Register Client Services & Audio Engine
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<IClientAuthService, ClientAuthService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AudioPlayerViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}