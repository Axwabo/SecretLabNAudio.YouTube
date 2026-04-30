using NAudio.Wave;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube;

public sealed record VideoIdPlaylistItem(VideoId Id) : PlaylistItem
{

    internal static StreamBasedFFmpegAudioProcessor CreateProvider(VideoId videoId, int sampleRate, int channels)
        => StreamBasedFFmpegAudioProcessor.Create(CreateYouTubeAudioProcessor.GetHighestQualityAsync(videoId), sampleRate, channels);

    public override ISampleProvider CreateProvider(int sampleRate, int channels)
        => CreateProvider(Id, sampleRate, channels);

}

public sealed record YouTubeVideoPlaylistItem(IVideo Video) : PlaylistItem(Video.Title)
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}

public sealed record CachedVideoIdPlaylistItem(VideoId Id) : CachedPlaylistItem<VideoId, string>(Id)
{

    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Id, sampleRate, channels);

}

public sealed record CachedYouTubeVideoPlaylistItem(IVideo Video) : CachedPlaylistItem<VideoId, string>(Video.Id, Video.Title)
{

    protected override AudioCacheBase<VideoId, string> GetCache() => YouTubeCache.Shared;

    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => VideoIdPlaylistItem.CreateProvider(Video.Id, sampleRate, channels);

}
