using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using LabApi.Loader.Features.Paths;
using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.FFmpeg.Interop;
using UnityEngine;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

public sealed class YouTubeCache : AudioCacheBase<VideoId, string>
{

    private static readonly FFmpegArguments Template = new()
    {
        Input = FFmpegArguments.StandardPipe,
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        OutputOptions = "-y"
    };

    public static YouTubeCache Shared { get; } = new(PathManager.Plugins.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.YouTube").CreateSubdirectory("Cache"));

    public YouTubeCache(string folder) : base(folder)
    {
    }

    public YouTubeCache(DirectoryInfo directoryInfo) : base(directoryInfo)
    {
    }

    protected override string GetKey(VideoId source) => source.Value;

    public async Awaitable<(string OutputPath, SaveCacheError? Error)> CacheAsync(VideoId id, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
    {
        if (id == default)
            return ("", new InvalidInputError(id.Value));
        var key = id.Value;
        var output = Output(key, optimizeFor);
        await Awaitable.BackgroundThreadAsync();
        await using var stream = await AudioPlayerExtensions.GetStream(id, cancellationToken);
        using var ffmpeg = FFmpegSL.Start(Template with {Output = output});
        if (ffmpeg == null)
            return (output, new FFmpegStartupError(FFmpegSL.LastCaughtStartError));
        try
        {
            await stream.CopyToAsync(ffmpeg.Stdin!.BaseStream, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return (output, e);
        }

        return !await ffmpeg.WaitForExitAsync(cancellationToken).ConfigureAwait(false)
            ? (output, SaveCacheError.Canceled)
            : ffmpeg.HasExitedWithError
                ? (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!))
                : (output, null);
    }

    public new bool TryGetPath(VideoId source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (source != default)
            return base.TryGetPath(source, out cachedPath);
        cachedPath = null;
        return false;
    }

}
