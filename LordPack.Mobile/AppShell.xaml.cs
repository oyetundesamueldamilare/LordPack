using LordPack.Mobile.Views;

namespace LordPack.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // --- Auth routes ---
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        // --- Bible navigation stack ---
        // BookListPage is the tab root (declared in XAML).
        // ChapterPickerPage and BiblePage are pushed on top of it.
        Routing.RegisterRoute("ChapterPickerPage", typeof(ChapterPickerPage));
        Routing.RegisterRoute("BiblePage", typeof(BiblePage));
    }
}