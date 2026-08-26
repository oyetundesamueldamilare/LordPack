using LordPack.Mobile.Interfaces;
using Plugin.Maui.Audio;

namespace LordPack.Mobile.Services;

public class AudioService : IAudioService
{
        private readonly IAudioManager _audioManager;
        private readonly IDownloadService _downloadService;
        private IAudioPlayer? _player;

        public AudioService(IAudioManager audioManager, IDownloadService downloadService)
        {
            _audioManager = audioManager;
            _downloadService = downloadService;
        }

        public async Task PlayAudioAsync(string remoteUrl, string fileName)
        {
            // 1. Resolve source: check if downloaded locally first
            string playbackSource;
            if (_downloadService.IsAudioDownloaded(fileName))
            {
                playbackSource = _downloadService.GetLocalFilePath(fileName);
                using var stream = File.OpenRead(playbackSource);
                _player = _audioManager.CreatePlayer(stream);
            }
            else
            {
                // Fallback to streaming directly from remote URL
                playbackSource = remoteUrl;
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(playbackSource);
                _player = _audioManager.CreatePlayer(stream);
            }

            _player.Play();
        }

        public bool IsPlaying => _player?.IsPlaying ?? false;
    public double CurrentPosition => _player?.CurrentPosition ?? 0;
    public double Duration => _player?.Duration ?? 0;

    public async Task InitializeAsync(string audioUrl)
    {
        using var httpClient = new HttpClient();
        var stream = await httpClient.GetStreamAsync(audioUrl);
        _player = _audioManager.CreatePlayer(stream);
    }


    public Task PauseAsync()
    {
        _player?.Pause();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _player?.Stop();
        return Task.CompletedTask;
    }

    public Task SeekAsync(double positionInSeconds)
    {
        if (_player != null && _player.CanSeek)
        {
            _player.Seek(positionInSeconds);
        }
        return Task.CompletedTask;
    }
}