using LordPack.Mobile.Interfaces;
using Plugin.Maui.Audio;

namespace LordPack.Mobile.Services;

public class AudioService : IAudioService
{
    private readonly IAudioManager _audioManager;
    private readonly IDownloadService _downloadService;
    private IAudioPlayer? _player;
    // Keep streams alive for the duration of playback
    private Stream? _activeStream;

    public AudioService(IAudioManager audioManager, IDownloadService downloadService)
    {
        _audioManager = audioManager;
        _downloadService = downloadService;
    }

    public bool IsPlaying => _player?.IsPlaying ?? false;
    public double CurrentPosition => _player?.CurrentPosition ?? 0;
    public double Duration => _player?.Duration ?? 0;

    public async Task InitializeAsync(string audioUrl)
    {
        DisposeCurrentPlayer();
        var httpClient = new HttpClient();
        _activeStream = await httpClient.GetStreamAsync(audioUrl);
        _player = _audioManager.CreatePlayer(_activeStream);
    }

    public async Task PlayAudioAsync(string remoteUrl, string fileName)
    {
        DisposeCurrentPlayer();

        if (_downloadService.IsAudioDownloaded(fileName))
        {
            // Offline path: open local file — keep FileStream open
            var localPath = _downloadService.GetLocalFilePath(fileName);
            _activeStream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _player = _audioManager.CreatePlayer(_activeStream);
        }
        else
        {
            // Online streaming path — keep HTTP stream open
            var httpClient = new HttpClient();
            _activeStream = await httpClient.GetStreamAsync(remoteUrl);
            _player = _audioManager.CreatePlayer(_activeStream);
        }

        _player.Play();
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

    private void DisposeCurrentPlayer()
    {
        _player?.Stop();
        _player?.Dispose();
        _player = null;

        _activeStream?.Dispose();
        _activeStream = null;
    }
}