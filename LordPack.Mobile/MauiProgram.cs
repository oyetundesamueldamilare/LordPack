using CommunityToolkit.Maui.Core;
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
        GlobalExceptionHandler.Initialize();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitCore()
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
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Configure HttpClient with SSL Bypass for local Debug builds
        builder.Services.AddHttpClient("LordPackApi", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .AddHttpMessageHandler<JwtAuthHandler>()
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("LordPackApi"));

        // Register Core Singletons
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<IAudioService, AudioService>();
        builder.Services.AddSingleton<LocalBibleDatabase>();

        // Typed HttpClients
        builder.Services.AddHttpClient<IClientAuthService, ClientAuthService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>()
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddHttpClient<IClientAudioBookService, ClientAudioBookService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddHttpClient<IDownloadService, DownloadService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddHttpClient<IPushNotificationService, PushNotificationService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        builder.Services.AddHttpClient<ITextBibleService, TextBibleService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => GetInsecureHandler());

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AudioPlayerViewModel>();
        builder.Services.AddTransient<BibleViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<BookListViewModel>();
        builder.Services.AddTransient<ChapterPickerViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AudioPlayerPage>();
        builder.Services.AddTransient<BiblePage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<BookListPage>();
        builder.Services.AddTransient<ChapterPickerPage>();

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