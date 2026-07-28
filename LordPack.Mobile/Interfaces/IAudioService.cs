using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Mobile.Interfaces
{
    public interface IAudioService
    {
        Task InitializeAsync(string audioUrl);
        Task PlayAsync();
        Task PauseAsync();
        Task StopAsync();
        Task SeekToAsync(double positionSeconds);
        bool IsPlaying { get; }
        double CurrentPosition { get; }
        double Duration { get; }
    }
}
