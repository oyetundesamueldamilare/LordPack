using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

[QueryProperty(nameof(BookId), "BookId")]
[QueryProperty(nameof(BookName), "BookName")]
[QueryProperty(nameof(TotalChapters), "TotalChapters")]
public partial class ChapterPickerViewModel : ObservableObject
{
    private readonly IClientAudioBookService _audioBookService;

    [ObservableProperty]
    private int _bookId;

    [ObservableProperty]
    private string _bookName = string.Empty;

    [ObservableProperty]
    private int _totalChapters;

    [ObservableProperty]
    private ObservableCollection<ChapterItem> _chapters = new();

    [ObservableProperty]
    private bool _isBusy;

    public ChapterPickerViewModel(IClientAudioBookService audioBookService)
    {
        _audioBookService = audioBookService;
    }

    /// <summary>Called after BookId is set via QueryProperty.</summary>
    partial void OnTotalChaptersChanged(int value)
    {
        Chapters = new ObservableCollection<ChapterItem>(
            Enumerable.Range(1, value).Select(n => new ChapterItem { Number = n })
        );
    }

    [RelayCommand]
    private async Task SelectChapterAsync(ChapterItem item)
    {
        if (item == null) return;

        // Try to resolve the Chapter's DB Id from the API
        IsBusy = true;
        try
        {
            // We navigate to BiblePage and pass BookId + ChapterNumber so BibleViewModel can resolve the API id
            var parameters = new Dictionary<string, object>
            {
                { "BookId", BookId },
                { "BookName", BookName },
                { "ChapterNumber", item.Number }
            };
            await Shell.Current.GoToAsync("BiblePage", parameters);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

/// <summary>Simple model representing a chapter number for the picker grid.</summary>
public class ChapterItem
{
    public int Number { get; set; }
    public override string ToString() => Number.ToString();
}
