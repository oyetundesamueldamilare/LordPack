using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

public partial class BibleViewModel : ObservableObject
{
    private readonly IClientAudioBookService _audioBookService;
    private readonly IAudioService _audioService;

    [RelayCommand]
    public async Task NextChapterAsync()
    {
        if (CurrentChapter == null) return;
        await LoadChapterAsync(CurrentChapter.ChapterNumber + 1);
    }

    [RelayCommand]
    public async Task PreviousChapterAsync()
    {
        if (CurrentChapter == null || CurrentChapter.ChapterNumber <= 1) return;
        await LoadChapterAsync(CurrentChapter.ChapterNumber - 1);
    }

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

    public BibleViewModel(IClientAudioBookService audioBookService, IAudioService audioService)
    {
        _audioBookService = audioBookService;
        _audioService = audioService;
    }

    [RelayCommand]
    public async Task LoadChapterAsync(int chapterId = 1)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var detail = await _audioBookService.GetChapterDetailsAsync(chapterId);
            if (detail != null)
            {
                CurrentChapter = detail;
                Verses = new ObservableCollection<VerseDto>(detail.Verses);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task PlayChapterAudioAsync()
    {
        if (CurrentChapter == null || string.IsNullOrEmpty(CurrentChapter.AudioUrl)) return;

        if (_audioService.IsPlaying)
        {
            await _audioService.PauseAsync();
            IsPlaying = false;
        }
        else
        {
            await _audioService.InitializeAsync(CurrentChapter.AudioUrl);
            await _audioService.PlayAsync();
            IsPlaying = true;
        }
    }

}