using LordPack.Mobile.ViewModels;

namespace LordPack.Mobile.Views;

public partial class BiblePage : ContentPage
{
    private readonly BibleViewModel _viewModel;

    public BiblePage(BibleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load chapter if not yet loaded (first appearance)
        if (_viewModel.CurrentChapter == null)
        {
            await _viewModel.LoadChapterAsync(1);
        }
        else
        {
            _viewModel.CheckDownloadStatus();
        }
    }
}