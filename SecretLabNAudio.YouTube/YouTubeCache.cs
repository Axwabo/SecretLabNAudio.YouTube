using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LabApi.Loader.Features.Paths;
using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.FFmpeg.Interop;
using UnityEngine;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

public sealed class YouTubeCache
{

    private static readonly FFmpegArguments Template = new()
    {
        Input = FFmpegArguments.StandardPipe,
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        OutputOptions = "-y"
    };

    public static YouTubeCache Shared { get; } = new(PathManager.Plugins.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.YouTube").CreateSubdirectory("Cache"));

    public string Folder { get; }

    public YouTubeCache(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(folder);
    }

    public YouTubeCache(DirectoryInfo directoryInfo) : this(directoryInfo.FullName)
    {
    }

    private string Output(string key, OptimizeFor optimizeFor) => Path.Combine(Folder, $"{key}.{optimizeFor.Extension}");

    public async Awaitable<(string OutputPath, SaveCacheError? Error)> CacheAsync(VideoId id, OptimizeFor optimizeFor)
    {
        if (id == default)
            return ("", new InvalidInputError(id.Value));
        var key = id.Value;
        var output = Output(key, optimizeFor);
        await Awaitable.BackgroundThreadAsync();
        await using var stream = await AudioPlayerExtensions.GetStream(id, CancellationToken.None);
        using var ffmpeg = FFmpegSL.Start(Template with {Output = output});
        if (ffmpeg == null)
            return (output, new FFmpegStartupError(FFmpegSL.LastCaughtStartError));
        await stream.CopyToAsync(ffmpeg.Stdin!.BaseStream);
        while (!ffmpeg.HasExited)
            await Task.Delay(100);
        ffmpeg.WaitForExit();
        return ffmpeg.HasExitedWithError ? (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!)) : (output, null);
    }

    public bool TryGetPath(VideoId source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (source == default)
        {
            cachedPath = null;
            return false;
        }

        var key = source.Value;
        var speed = Output(key, OptimizeFor.ReadingSpeed);
        if (File.Exists(speed))
        {
            cachedPath = speed;
            return true;
        }

        var size = Output(key, OptimizeFor.FileSize);
        if (File.Exists(size))
        {
            cachedPath = size;
            return true;
        }

        cachedPath = null;
        return false;
    }

}
