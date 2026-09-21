using NAudio.Wave;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube;

/// <summary>
/// A queued playlist item that plays a YouTube video based on its ID.
/// </summary>
/// <param name="Source">The ID of the video to play.</param>
public sealed record VideoIdPlaylistItem(VideoId Source) : PlaylistItem, IVideoIdPlaylistItem
{

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> that resolves the highest-quality audio stream of the YouTube video.
    /// </summary>
    /// <param name="videoId">The ID of the video to play.</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <remarks>By default, <see cref="AsyncFFmpegProcessorBase.SleepThresholdSeconds"/> will be set to 75% of the capacity.</remarks>
    public static StreamBasedFFmpegAudioProcessor CreateProvider(VideoId videoId, int sampleRate, int channels, double capacity = AsyncFFmpegProcessorBase.DefaultCapacity)
        => StreamBasedFFmpegAudioProcessor.Create(CreateYouTubeAudioProcessor.GetHighestQualityAsync(videoId), sampleRate, channels, capacity);

    /// <inheritdoc />
    protected override ISampleProvider CreateProvider(int sampleRate, int channels)
        => CreateProvider(Source, sampleRate, channels);

}

/// <summary>
/// A queued playlist item that plays a YouTube video based on its metadata.
/// </summary>
/// <param name="Video">The video to play.</param>
public sealed record YouTubeVideoPlaylistItem(IVideo Video) : PlaylistItem(Video.Title), IYouTubeVideoPlaylistItem
{

    /// <inheritdoc />
    protected override ISampleProvider CreateProvider(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}

/// <summary>
/// A queued playlist item that plays the cached version of a YouTube video if available.
/// </summary>
/// <param name="Source">The ID of the video to play.</param>
public sealed record CachedVideoIdPlaylistItem(VideoId Source) : CachedPlaylistItem<VideoId, string>(Source), IVideoIdPlaylistItem
{

    /// <inheritdoc />
    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    /// <inheritdoc />
    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Source, sampleRate, channels);

}

/// <summary>
/// A queued playlist item that plays the cached version of a YouTube video if available.
/// </summary>
/// <param name="Video">The video to play.</param>
public sealed record CachedYouTubeVideoPlaylistItem(IVideo Video) : CachedPlaylistItem<VideoId, string>(Video.Id, Video.Title), IYouTubeVideoPlaylistItem
{

    /// <inheritdoc />
    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    /// <inheritdoc />
    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}
