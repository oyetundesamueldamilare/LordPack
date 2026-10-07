using LordPack.Mobile.Interfaces;

namespace LordPack.Mobile.Services;

public class DownloadService : IDownloadService
{
    private readonly HttpClient _httpClient;

    public DownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string GetLocalFilePath(string fileName)
    {
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    }

    public bool IsAudioDownloaded(string fileName)
    {
        var localPath = GetLocalFilePath(fileName);
        return File.Exists(localPath);
    }

    public async Task<string?> DownloadAudioAsync(
        string remoteUrl,
        string fileName,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var localPath = GetLocalFilePath(fileName);

        if (File.Exists(localPath)) return localPath;

        try
        {
            using var response = await _httpClient.GetAsync(remoteUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            var totalBytesRead = 0L;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalBytesRead += bytesRead;

                if (totalBytes != -1L)
                {
                    progress?.Report((double)totalBytesRead / totalBytes);
                }
            }

            return localPath;
        }
        catch
        {
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            return null;
        }
    }

    public bool DeleteDownloadedAudio(string fileName)
    {
        var localPath = GetLocalFilePath(fileName);
        if (File.Exists(localPath))
        {
            File.Delete(localPath);
            return true;
        }
        return false;
    }
}