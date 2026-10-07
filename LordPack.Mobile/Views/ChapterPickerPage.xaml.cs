using LordPack.Mobile.ViewModels;

namespace LordPack.Mobile.Views;

public partial class ChapterPickerPage : ContentPage
{
    public ChapterPickerPage(ChapterPickerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
