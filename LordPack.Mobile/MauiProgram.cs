using LordPack.Mobile.Handlers;
using LordPack.Mobile.Interfaces;
using LordPack.Mobile.Services;
using LordPack.Mobile.ViewModels;
using LordPack.Mobile.Views;
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

        // Safe cross-platform base URL assignment
        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7147/"
            : "https://localhost:7147/";

        builder.Services.AddTransient<JwtAuthHandler>();

        // Configure HttpClient with SSL Bypass for local Debug builds
        builder.Services.AddHttpClient("LordPackApi", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .AddHttpMessageHandler<JwtAuthHandler>()
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("LordPackApi"));

        // Register Core Services
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<IClientAuthService, ClientAuthService>();
        builder.Services.AddSingleton<IClientAudioBookService, ClientAudioBookService>();

        // Download Service & Offline Caching
        builder.Services.AddHttpClient<IDownloadService, DownloadService>();

        // Register ViewModels & Pages
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AudioPlayerViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AudioPlayerPage>();
        builder.Services.AddTransient<BibleViewModel>();
        builder.Services.AddTransient<BiblePage>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();
        // Register DelegatingHandler
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Register ViewModels & Pages
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();



        builder.Services.AddHttpClient<IClientAuthService, ClientAuthService>(client =>
        {
            client.BaseAddress = new Uri(
                DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7147/"
                    : "https://localhost:7147/");
        })
 .AddHttpMessageHandler<AuthHeaderHandler>()
 .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static HttpMessageHandler GetInsecureHandler()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
        return handler;
    }
}