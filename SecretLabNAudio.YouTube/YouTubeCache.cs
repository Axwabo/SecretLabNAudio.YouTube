using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using LabApi.Loader.Features.Paths;
using SecretLabNAudio.FFmpeg;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.FFmpeg.Interop;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

/// <summary>
/// A cache that downloads and optimizes YouTube videos.
/// </summary>
public sealed class YouTubeCache : AudioCacheBase<VideoId, string>
{

    private static readonly FFmpegArguments Template = SimpleFileCache.ArgumentsTemplate.ReadFromStandardInput();

    /// <summary>
    /// A shared <see cref="YouTubeCache"/> instance.
    /// </summary>
    public static YouTubeCache Shared { get; } = new(
        PathManager.Plugins.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.YouTube").CreateSubdirectory("Cache"),
        YoutubeClient.Shared
    );

    private readonly YoutubeClient _client;

    /// <summary>
    /// Initializes a new cache, and creates the directory if necessary.
    /// </summary>
    /// <param name="folder">The directory to save files to.</param>
    /// <param name="client">The client to use when connecting to YouTube.</param>
    public YouTubeCache(string folder, YoutubeClient client) : base(folder) => _client = client;

    /// <summary>
    /// Initializes a new cache, and creates the directory if necessary.
    /// </summary>
    /// <param name="directoryInfo">The directory to save files to.</param>
    /// <param name="client">The client to use when connecting to YouTube.</param>
    public YouTubeCache(DirectoryInfo directoryInfo, YoutubeClient client) : base(directoryInfo) => _client = client;

    /// <inheritdoc/>
    public override string GetKey(VideoId source) => source.Value;

    public override async Awaitable<(string OutputPath, SaveCacheError? Error)> CacheAsync(VideoId id, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
    {
        if (id == default)
            return ("", new InvalidInputError(id.Value));
        var key = id.Value;
        var output = GetOutput(key, optimizeFor);
        await Awaitable.BackgroundThreadAsync();
        await using var stream = await _client.GetAudioStreamAsync(id, cancellationToken).ConfigureAwait(false);
        if (stream == null)
            return (output, new StreamUnavailableError(id));
        using var ffmpeg = FFmpegSL.Start(Template with {Output = output});
        if (ffmpeg == null)
            return (output, FFmpegSL.LastCaughtStartError);
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

    /// <inheritdoc/>
    public override bool TryGetPath(VideoId source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (source != default)
            return base.TryGetPath(source, out cachedPath);
        cachedPath = null;
        return false;
    }

}
