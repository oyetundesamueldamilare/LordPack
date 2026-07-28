using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LordPack.Api.Interfaces;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
using System.Collections.ObjectModel;

namespace LordPack.Mobile.ViewModels;

public partial class AudioPlayerViewModel : ObservableObject
{
    private readonly IClientAudioBookService _audioBookService;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<AudioBookDto> _audioBooks = new();

    [ObservableProperty]
    private AudioBookDto? _selectedAudioBook;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isBusy;

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
    }

    [RelayCommand]
    private async Task TogglePlayPauseAsync()
    {
        if (SelectedAudioBook == null) return;

        if (_audioService.IsPlaying)
        {
            await _audioService.PauseAsync();
            IsPlaying = false;
        }
        else
        {
            await _audioService.PlayAsync();
            IsPlaying = true;
        }
    }
}