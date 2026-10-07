using LordPack.Mobile.ViewModels;

namespace LordPack.Mobile.Views;

public partial class BookListPage : ContentPage
{
    private readonly BookListViewModel _viewModel;

    public BookListPage(BookListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.Books.Any())
        {
            await _viewModel.LoadBooksAsync();
        }
    }
}
