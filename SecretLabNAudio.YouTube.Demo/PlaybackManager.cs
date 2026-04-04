using System.Collections.Concurrent;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.YouTube.Caches;
using SecretLabNAudio.YouTube.Extensions;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube.Demo;

public static class PlaybackManager
{

    private static readonly ConcurrentDictionary<VideoId, bool> CachingInProgress = [];

    private static readonly SpeakerSettings Settings = SpeakerSettings.GloballyAudible with {Volume = 0.2f};

    private static AudioPlayer? _player;

    public static void Play(VideoId id)
    {
        PlayInternal(id);
        _ = BroadcastNowPlayingAsync(id);
    }

    public static void Play(VideoSearchResult searchResult)
    {
        PlayInternal(searchResult.Id);
        Server.SendBroadcast($"Now playing: {searchResult.Title}\nby {searchResult.Author}", 10);
    }

    private static void PlayInternal(VideoId videoId)
    {
        if (_player == null || AudioPlayerPool.IsPooled(_player))
            _player = AudioPlayerPool.Rent(Settings).PoolOnEnd();
        _player.UseCachedYouTube(videoId);
        if (!YouTubeCache.Shared.TryGetPath(videoId, out _) && !CachingInProgress.ContainsKey(videoId))
            _ = CacheAsync(videoId);
    }

    private static async Awaitable CacheAsync(VideoId videoId)
    {
        CachingInProgress[videoId] = true;
        try
        {
            await YouTubeCache.Shared.CacheAsync(videoId, OptimizeFor.FileSize);
        }
        finally
        {
            CachingInProgress.TryRemove(videoId, out _);
        }
    }

    private static async Awaitable BroadcastNowPlayingAsync(VideoId id)
    {
        var video = await YoutubeClient.Shared.Videos.GetAsync(id);
        Server.SendBroadcast($"Now playing: {video.Title}\nby {video.Author}", 10);
    }

}
