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

        // Only load if not already loaded to prevent refetching when switching tabs
        if (_viewModel.CurrentChapter == null)
        {
            await _viewModel.LoadChapterCommand.ExecuteAsync(1);
        }
    }
}