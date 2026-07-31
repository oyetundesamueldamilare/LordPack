using LordPack.Mobile.ViewModels;

namespace LordPack.Mobile.Views;

public partial class AudioPlayerPage : ContentPage
{
    public AudioPlayerPage(AudioPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}