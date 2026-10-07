using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

public partial class BookListViewModel : ObservableObject
{
    private readonly ITextBibleService _textBibleService;

    [ObservableProperty]
    private ObservableCollection<BibleBookDto> _books = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _showOldTestament = true;

    public BookListViewModel(ITextBibleService textBibleService)
    {
        _textBibleService = textBibleService;
    }

    [RelayCommand]
    public async Task LoadBooksAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var testament = ShowOldTestament ? "OldTestament" : "NewTestament";
            var result = await _textBibleService.GetBooksAsync(testament);
            Books = new ObservableCollection<BibleBookDto>(result);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleTestamentAsync()
    {
        ShowOldTestament = !ShowOldTestament;
        await LoadBooksAsync();
    }

    [RelayCommand]
    private async Task SelectBookAsync(BibleBookDto book)
    {
        if (book == null) return;
        // Navigate to ChapterPickerPage with book info as query param
        var parameters = new Dictionary<string, object>
        {
            { "BookId", book.Id },
            { "BookName", book.Name },
            { "TotalChapters", book.TotalChapters }
        };
        await Shell.Current.GoToAsync("ChapterPickerPage", parameters);
    }
}
