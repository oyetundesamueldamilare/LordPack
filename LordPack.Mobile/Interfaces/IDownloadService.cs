namespace LordPack.Mobile.Interfaces;

public interface IDownloadService
{
    Task<string?> DownloadAudioAsync(string remoteUrl, string fileName, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    bool IsAudioDownloaded(string fileName);
    string GetLocalFilePath(string fileName);
    bool DeleteDownloadedAudio(string fileName);
}