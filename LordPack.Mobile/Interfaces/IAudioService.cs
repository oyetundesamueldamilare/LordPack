using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Mobile.Interfaces
{
    public interface IAudioService
    {
        Task InitializeAsync(string audioUrl);
        Task PlayAudioAsync(string remoteUrl, string fileName);
        Task PauseAsync();
        Task StopAsync();
        Task SeekAsync(double positionSeconds);
        bool IsPlaying { get; }
        double CurrentPosition { get; }
        double Duration { get; }
    }

}
