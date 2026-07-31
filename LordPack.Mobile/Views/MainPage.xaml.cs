using LordPack.Mobile.ViewModels;

namespace LordPack.Mobile.Views;

public partial class MainPage : ContentPage
{
    private readonly AudioPlayerViewModel _viewModel;

    public MainPage(AudioPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAudioBooksCommand.ExecuteAsync(null);
    }
}