using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Api.Interfaces;
using LordPack.Mobile.Interfaces;
using LordPack.Mobile.Services;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

public partial class AudioPlayerViewModel : ObservableObject
{
    private readonly IClientAudioBookService _audioBookService;
    private readonly IAudioService _audioService;
    private System.Threading.Timer? _progressTimer;

    // ✅ Clean backing fields without [ObservableProperty]
    private ObservableCollection<AudioBookDto> _audioBooks = new();
    public ObservableCollection<AudioBookDto> AudioBooks
    {
        get => _audioBooks;
        set => SetProperty(ref _audioBooks, value);
    }

    private AudioBookDto? _selectedAudioBook;
    public AudioBookDto? SelectedAudioBook
    {
        get => _selectedAudioBook;
        set => SetProperty(ref _selectedAudioBook, value);
    }

    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set => SetProperty(ref _isPlaying, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private double _currentPosition;
    public double CurrentPosition
    {
        get => _currentPosition;
        set => SetProperty(ref _currentPosition, value);
    }

    private double _duration;
    public double Duration
    {
        get => _duration;
        set => SetProperty(ref _duration, value);
    }

    private string _formattedCurrentPosition = "00:00";
    public string FormattedCurrentPosition
    {
        get => _formattedCurrentPosition;
        set => SetProperty(ref _formattedCurrentPosition, value);
    }

    private string _formattedDuration = "00:00";
    public string FormattedDuration
    {
        get => _formattedDuration;
        set => SetProperty(ref _formattedDuration, value);
    }

    public AudioPlayerViewModel(IClientAudioBookService audioBookService, IAudioService audioService)
    {
        _audioBookService = audioBookService;
        _audioService = audioService;
    }

    [RelayCommand]
    private async Task LoadAudioBooksAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        var items = await _audioBookService.GetAudioBooksAsync();
        AudioBooks = new ObservableCollection<AudioBookDto>(items);

        IsBusy = false;
    }

    [RelayCommand]
    private async Task PlayAudioAsync(AudioBookDto audioBook)
    {
        if (audioBook == null) return;

        SelectedAudioBook = audioBook;
        await _audioService.InitializeAsync(audioBook.AudioUrl);
        await _audioService.PlayAsync();

        IsPlaying = true;
        Duration = _audioService.Duration;
        FormattedDuration = TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");

        StartProgressTimer();
    }

    [RelayCommand]
    private async Task TogglePlayPauseAsync()
    {
        if (SelectedAudioBook == null) return;

        if (_audioService.IsPlaying)
        {
            await _audioService.PauseAsync();
            IsPlaying = false;
            StopProgressTimer();
        }
        else
        {
            await _audioService.PlayAsync();
            IsPlaying = true;
            StartProgressTimer();
        }
    }

    [RelayCommand]
    private async Task SeekAsync(double newPosition)
    {
        await _audioService.SeekAsync(newPosition);
        CurrentPosition = newPosition;
        FormattedCurrentPosition = TimeSpan.FromSeconds(CurrentPosition).ToString(@"mm\:ss");
    }

    [RelayCommand]
    private async Task SkipForwardAsync()
    {
        double target = Math.Min(CurrentPosition + 10, Duration);
        await SeekAsync(target);
    }

    [RelayCommand]
    private async Task SkipBackwardAsync()
    {
        double target = Math.Max(CurrentPosition - 10, 0);
        await SeekAsync(target);
    }

    private void StartProgressTimer()
    {
        _progressTimer?.Dispose();
        _progressTimer = new System.Threading.Timer(_ =>
        {
            if (_audioService.IsPlaying)
            {
                CurrentPosition = _audioService.CurrentPosition;
                FormattedCurrentPosition = TimeSpan.FromSeconds(CurrentPosition).ToString(@"mm\:ss");
            }
        }, null, 0, 500);
    }

    private void StopProgressTimer()
    {
        _progressTimer?.Dispose();
        _progressTimer = null;
    }
}