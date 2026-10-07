using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

/// <summary>
/// ViewModel for the Scripture reader (BiblePage).
/// Receives BookId + ChapterNumber via Shell QueryProperty navigation
/// from ChapterPickerPage, resolves the chapter API Id, loads verses,
/// and syncs audio playback position to the active verse.
/// </summary>
[QueryProperty(nameof(BookId), "BookId")]
[QueryProperty(nameof(BookName), "BookName")]
[QueryProperty(nameof(ChapterNumber), "ChapterNumber")]
public partial class BibleViewModel : ObservableObject
{
    private readonly IClientAudioBookService _audioBookService;
    private readonly IAudioService _audioService;
    private readonly IDownloadService _downloadService;
    private readonly ITextBibleService _textBibleService;

    // ── Query Parameters (set by Shell navigation) ─────────────────────────

    [ObservableProperty]
    private int _bookId;

    [ObservableProperty]
    private string _bookName = string.Empty;

    private int _chapterNumber;
    public int ChapterNumber
    {
        get => _chapterNumber;
        set
        {
            if (SetProperty(ref _chapterNumber, value) && value > 0)
            {
                // Triggered when navigation sets this property — auto-load
                _ = LoadChapterByBookAndNumberAsync(_bookId, value);
            }
        }
    }

    // ── Chapter & Verse Data ───────────────────────────────────────────────

    private ChapterDetailDto? _currentChapter;
    public ChapterDetailDto? CurrentChapter
    {
        get => _currentChapter;
        set => SetProperty(ref _currentChapter, value);
    }

    private ObservableCollection<VerseDto> _verses = new();
    public ObservableCollection<VerseDto> Verses
    {
        get => _verses;
        set => SetProperty(ref _verses, value);
    }

    // ── UI State ───────────────────────────────────────────────────────────

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set => SetProperty(ref _isPlaying, value);
    }

    private bool _isDownloading;
    public bool IsDownloading
    {
        get => _isDownloading;
        set => SetProperty(ref _isDownloading, value);
    }

    private double _downloadProgress;
    public double DownloadProgress
    {
        get => _downloadProgress;
        set => SetProperty(ref _downloadProgress, value);
    }

    private bool _isAudioDownloaded;
    public bool IsAudioDownloaded
    {
        get => _isAudioDownloaded;
        set => SetProperty(ref _isAudioDownloaded, value);
    }

    // ── Font & Theme ───────────────────────────────────────────────────────

    private double _fontSize = 16;
    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    private bool _isDarkTheme = true;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            SetProperty(ref _isDarkTheme, value);
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(VerseTextColor));
        }
    }

    public string BackgroundColor => IsDarkTheme ? "#0F172A" : "#FFFFFF";
    public string VerseTextColor => IsDarkTheme ? "#E2E8F0" : "#1E293B";

    // ── Audio ↔ Text Sync ──────────────────────────────────────────────────

    private int _activeVerseIndex = -1;
    public int ActiveVerseIndex
    {
        get => _activeVerseIndex;
        set => SetProperty(ref _activeVerseIndex, value);
    }

    private System.Threading.Timer? _syncTimer;

    // ── Constructor ────────────────────────────────────────────────────────

    public BibleViewModel(
        IClientAudioBookService audioBookService,
        IAudioService audioService,
        IDownloadService downloadService,
        ITextBibleService textBibleService)
    {
        _audioBookService = audioBookService;
        _audioService = audioService;
        _downloadService = downloadService;
        _textBibleService = textBibleService;
    }

    // ── Commands ───────────────────────────────────────────────────────────

    /// <summary>
    /// Loads a chapter directly by its DB/API Id.
    /// Called by BiblePage.OnAppearing for the default load (Chapter Id = 1).
    /// </summary>
    [RelayCommand]
    public async Task LoadChapterAsync(int chapterId = 1)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var detail = await _textBibleService.GetChapterDetailAsync(chapterId);
            if (detail != null)
            {
                CurrentChapter = detail;
                if (!string.IsNullOrEmpty(detail.BookName))
                    BookName = detail.BookName;
                Verses = new ObservableCollection<VerseDto>(detail.Verses);
                CheckDownloadStatus();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads a chapter by BookId + ordinal ChapterNumber.
    /// Called when navigated from ChapterPickerPage.
    /// Fetches book chapters list to resolve the correct API Id.
    /// </summary>
    public async Task LoadChapterByBookAndNumberAsync(int bookId, int chapterNumber)
    {
        if (IsBusy || bookId <= 0 || chapterNumber <= 0) return;
        IsBusy = true;

        try
        {
            var chapters = await _audioBookService.GetChaptersByBookIdAsync(bookId);
            var target = chapters?.FirstOrDefault(c => c.ChapterNumber == chapterNumber);

            int apiId = target?.Id ?? chapterNumber;

            var detail = await _textBibleService.GetChapterDetailAsync(apiId);
            if (detail != null)
            {
                if (string.IsNullOrEmpty(detail.BookName) && !string.IsNullOrEmpty(BookName))
                    detail.BookName = BookName;

                CurrentChapter = detail;
                Verses = new ObservableCollection<VerseDto>(detail.Verses);
                CheckDownloadStatus();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task NextChapterAsync()
    {
        if (CurrentChapter == null) return;
        await LoadChapterByBookAndNumberAsync(BookId > 0 ? BookId : 1, CurrentChapter.ChapterNumber + 1);
    }

    [RelayCommand]
    public async Task PreviousChapterAsync()
    {
        if (CurrentChapter == null || CurrentChapter.ChapterNumber <= 1) return;
        await LoadChapterByBookAndNumberAsync(BookId > 0 ? BookId : 1, CurrentChapter.ChapterNumber - 1);
    }

    [RelayCommand]
    public async Task PlayChapterAudioAsync()
    {
        if (CurrentChapter == null || string.IsNullOrEmpty(CurrentChapter.AudioUrl)) return;

        var fileName = GetAudioFileName();

        if (_audioService.IsPlaying)
        {
            await _audioService.PauseAsync();
            IsPlaying = false;
            StopSyncTimer();
        }
        else
        {
            await _audioService.PlayAudioAsync(CurrentChapter.AudioUrl, fileName);
            IsPlaying = true;
            StartSyncTimer();
        }
    }

    [RelayCommand]
    public async Task DownloadChapterAudioAsync()
    {
        if (CurrentChapter == null || IsDownloading || string.IsNullOrEmpty(CurrentChapter.AudioUrl)) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Offline", "An active internet connection is required to download audio.", "OK");
            return;
        }

        IsDownloading = true;
        DownloadProgress = 0;

        var fileName = GetAudioFileName();
        var progress = new Progress<double>(p => DownloadProgress = p);

        var resultPath = await _downloadService.DownloadAudioAsync(
            CurrentChapter.AudioUrl,
            fileName,
            progress);

        IsDownloading = false;

        if (!string.IsNullOrEmpty(resultPath))
        {
            IsAudioDownloaded = true;
        }
        else
        {
            await Shell.Current.DisplayAlert("Download Failed", "Could not complete the download. Please try again.", "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteDownloadedAudioAsync()
    {
        if (CurrentChapter == null) return;

        var fileName = GetAudioFileName();
        if (_downloadService.DeleteDownloadedAudio(fileName))
        {
            IsAudioDownloaded = false;
            DownloadProgress = 0;
        }
    }

    [RelayCommand]
    private void IncreaseFontSize() => FontSize = Math.Min(FontSize + 2, 28);

    [RelayCommand]
    private void DecreaseFontSize() => FontSize = Math.Max(FontSize - 2, 12);

    [RelayCommand]
    private void ToggleTheme() => IsDarkTheme = !IsDarkTheme;

    // ── Helpers ────────────────────────────────────────────────────────────

    public void CheckDownloadStatus()
    {
        if (CurrentChapter == null) return;
        var fileName = GetAudioFileName();
        IsAudioDownloaded = _downloadService.IsAudioDownloaded(fileName);
    }

    private string GetAudioFileName()
    {
        if (CurrentChapter == null) return string.Empty;
        var sanitizedBookName = (CurrentChapter.BookName.Length > 0
            ? CurrentChapter.BookName
            : BookName).Replace(" ", "_").ToLowerInvariant();
        return $"{sanitizedBookName}_{CurrentChapter.ChapterNumber}.mp3";
    }

    // ── Audio ↔ Text Sync Timer ────────────────────────────────────────────

    private void StartSyncTimer()
    {
        _syncTimer?.Dispose();
        _syncTimer = new System.Threading.Timer(_ =>
        {
            if (!_audioService.IsPlaying || CurrentChapter == null) return;

            var pos = _audioService.CurrentPosition;
            int verseCount = Verses.Count;
            if (verseCount == 0) return;

            double duration = _audioService.Duration;
            if (duration <= 0) return;

            int estimatedIndex = (int)((pos / duration) * verseCount);
            estimatedIndex = Math.Clamp(estimatedIndex, 0, verseCount - 1);

            if (estimatedIndex != ActiveVerseIndex)
            {
                MainThread.BeginInvokeOnMainThread(() => ActiveVerseIndex = estimatedIndex);
            }

        }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(750));
    }

    private void StopSyncTimer()
    {
        _syncTimer?.Dispose();
        _syncTimer = null;
    }
}