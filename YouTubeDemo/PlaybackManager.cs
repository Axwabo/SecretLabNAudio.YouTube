using System;
using System.Collections.Concurrent;
using System.Threading;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Caches;
using SecretLabNAudio.YouTube.Extensions;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Videos;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.YouTube.Demo;

public static class PlaybackManager
{

    private static readonly ConcurrentDictionary<VideoId, bool> CachingInProgress = [];

    private static readonly SpeakerSettings Settings = SpeakerSettings.GloballyAudible with {Volume = 0.5f};

    private static AudioPlayer? _player;

    private static string? _lastError;

    private static bool IsInactive => _player == null || AudioPlayerPool.IsPooled(_player);

    public static string? Error => !IsInactive
        ? _player!.MapError()
        : _lastError == null
            ? "No active playback"
            : $"Last error: {_lastError}";

    public static bool? TogglePause() => IsInactive ? null : _player!.IsPaused = !_player.IsPaused;

    public static bool Stop()
    {
        if (IsInactive)
            return false;
        _lastError = _player!.MapError();
        AudioPlayerPool.Return(_player!);
        return true;
    }

    public static void Play(VideoId id)
    {
        PlayInternal(id);
        _ = BroadcastNowPlayingAsync(id);
    }

    public static void Play(IVideo video)
    {
        PlayInternal(video.Id, video);
        BroadcastNowPlaying(video.Title, video.Author.ChannelTitle);
    }

    private static void PlayInternal(VideoId videoId, IVideo? video = null)
    {
        _lastError = null;
        EnsurePlayer()
            .UseCachedYouTube(videoId)
            .Resume();
        if (!YouTubeCache.Shared.IsCached(videoId) && !CachingInProgress.ContainsKey(videoId))
            _ = CacheAsync(videoId, video);
    }

    private static AudioPlayer EnsurePlayer()
    {
        if (!IsInactive)
            return _player!;
        _player = AudioPlayerPool.Rent(Settings);
        _player.Ended += OnEnded;
        return _player;
    }

    private static void OnEnded()
    {
        _lastError = _player!.MapError();
        AudioPlayerPool.Return(_player!);
    }

    private static async Awaitable CacheAsync(VideoId videoId, IVideo? video)
    {
        CachingInProgress[videoId] = true;
        await Awaitable.NextFrameAsync();
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        Logger.Info($"Caching YouTube video {videoId}");
        var (_, error) = video == null
            ? await YouTubeCache.Shared.CacheAsync(videoId, OptimizeFor.FileSize, cts.Token)
            : await YouTubeCache.Shared.CacheAsync(video, OptimizeFor.FileSize, cts.Token);
        if (error is not null)
            Logger.Error($"Failed to cache video {videoId}: {error.ToHumanReadableString()}");
        else
            Logger.Info($"Successfully cached YouTube video {videoId}");
        CachingInProgress.TryRemove(videoId, out _);
    }

    private static void BroadcastNowPlaying(string title, string channelTitle) => Server.SendBroadcast($"Now playing: {title}\nby {channelTitle}", 10);

    private static async Awaitable BroadcastNowPlayingAsync(VideoId id)
    {
        if (YouTubeCache.Shared.GetCachedMetadata(id) is ({ } videoTitle, { } channelTitle, _))
        {
            BroadcastNowPlaying(videoTitle, channelTitle);
            return;
        }

        try
        {
            var video = await YoutubeClient.Shared.Videos.GetAsync(id);
            Server.SendBroadcast($"Now playing: {video.Title}\nby {video.Author}", 10);
            _ = YouTubeCache.Shared.WriteMetadataAsync(video);
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to fetch video metadata for video {id}:\n{e}");
            Server.SendBroadcast($"Now playing YouTube video {id}", 10);
        }
    }

    private static string? MapError(this AudioPlayer player) => player.SourceAs<AsyncFFmpegProcessorBase>()?.MapError<string?>(
        code => $"FFmpeg startup error: 0x{code:X8}",
        exception => exception.ToString(),
        error => error,
        () => null
    );

}
