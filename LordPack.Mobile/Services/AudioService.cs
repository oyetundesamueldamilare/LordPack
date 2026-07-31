using LordPack.Mobile.Interfaces;
using Plugin.Maui.Audio;

namespace LordPack.Mobile.Services;

public class AudioService : IAudioService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _player;

    public AudioService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
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

    public Task PlayAsync()
    {
        _player?.Play();
        return Task.CompletedTask;
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