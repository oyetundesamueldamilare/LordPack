using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Mobile.Views;

namespace LordPack.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IClientAuthService _authService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private bool _isGuest;

    [ObservableProperty]
    private bool _isBusy;

    public ProfileViewModel(IClientAuthService authService)
    {
        _authService = authService;
    }

    public async Task LoadUserProfileAsync()
    {
        IsBusy = true;
        var session = await _authService.GetCurrentSessionAsync();

        if (session != null)
        {
            FullName = session.IsGuest ? "Guest User" : session.FullName;
            Email = session.Email;
            IsGuest = session.IsGuest;
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task UpgradeAccountAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}