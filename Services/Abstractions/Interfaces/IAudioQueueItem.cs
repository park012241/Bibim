using HyperLapse.Bibim.Service.Abstractions.EventArgs;

namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IAudioQueueItem
{
    public event EventHandler<AudioQueueItemStateChangedEventArgs>? StateChanged;

    public string SourceDisplayName { get; }
    public string DisplayName { get; }
    public CancellationToken CancellationToken { get; }
    public TaskCompletionSource? TaskCompletionSource { get; }

    /// <summary>
    /// Returns the audio stream with any format that ffmpeg can play.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Stream> GetAudioStreamAsync(CancellationToken cancellationToken);
}