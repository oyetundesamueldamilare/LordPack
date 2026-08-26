using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;

namespace LordPack.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(IClientAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Call from LoginPage.xaml.cs OnAppearing to bypass login if already authenticated.
    /// </summary>
    public async Task CheckExistingSessionAsync()
    {
        if (await _authService.IsAuthenticatedAsync())
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both email and password.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        var success = await _authService.LoginAsync(new LoginRequestDto
        {
            Email = Email.Trim(),
            Password = Password
        });

        IsBusy = false;

        if (success)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            ErrorMessage = "Invalid email or password.";
        }
    }

    [RelayCommand]
    private async Task LoginAsGuestAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        var success = await _authService.LoginAsGuestAsync();

        IsBusy = false;

        if (success)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            ErrorMessage = "Could not start guest session. Ensure API is running.";
        }
    }
}