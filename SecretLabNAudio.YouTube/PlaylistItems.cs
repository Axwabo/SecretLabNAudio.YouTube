using NAudio.Wave;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube;

public sealed record VideoIdPlaylistItem(VideoId Source) : PlaylistItem, IVideoIdPlaylistItem
{

    internal static StreamBasedFFmpegAudioProcessor CreateProvider(VideoId videoId, int sampleRate, int channels)
        => StreamBasedFFmpegAudioProcessor.Create(CreateYouTubeAudioProcessor.GetHighestQualityAsync(videoId), sampleRate, channels);

    protected override ISampleProvider CreateProvider(int sampleRate, int channels)
        => CreateProvider(Source, sampleRate, channels);

}

public sealed record YouTubeVideoPlaylistItem(IVideo Video) : PlaylistItem(Video.Title), IYouTubeVideoPlaylistItem
{

    protected override ISampleProvider CreateProvider(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}

public sealed record CachedVideoIdPlaylistItem(VideoId Source) : CachedPlaylistItem<VideoId, string>(Source), IVideoIdPlaylistItem
{

    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Source, sampleRate, channels);

}

public sealed record CachedYouTubeVideoPlaylistItem(IVideo Video) : CachedPlaylistItem<VideoId, string>(Video.Id, Video.Title), IYouTubeVideoPlaylistItem
{

    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}
