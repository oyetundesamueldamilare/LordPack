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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.AudioBooks.Any())
        {
            _viewModel.LoadAudioBooksCommand.Execute(null);
        }
    }
}