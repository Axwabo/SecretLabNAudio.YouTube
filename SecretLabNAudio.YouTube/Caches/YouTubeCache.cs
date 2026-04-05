using LabApi.Loader.Features.Paths;
using SecretLabNAudio.FFmpeg;
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.FFmpeg.Interop;
using SecretLabNAudio.YouTube.Extensions;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;

namespace SecretLabNAudio.YouTube.Caches;

/// <summary>
/// A cache that downloads and optimizes YouTube videos.
/// </summary>
public sealed class YouTubeCache : AudioCacheBase<VideoId, string>
{

    private const string TitleExtension = "title";
    private const string AuthorExtension = "author";

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

    /// <summary>
    /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
    /// </summary>
    /// <param name="id">The ID of the video to download.</param>
    /// <param name="optimizeFor">What to optimize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
    public override Awaitable<SaveCacheResult> CacheAsync(VideoId id, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
        => CacheAsync(id, null, null, PickStream.HighestBitrate, optimizeFor, cancellationToken);

    /// <summary>
    /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
    /// </summary>
    /// <param name="id">The ID of the video to download.</param>
    /// <param name="title">The title of the video (if known).</param>
    /// <param name="author">The author of the video (if known).</param>
    /// <param name="pickStream">A delegate that picks the most optimal stream from, given the manifest.</param>
    /// <param name="optimizeFor">What to optimize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
    public async Awaitable<SaveCacheResult> CacheAsync(VideoId id, string? title, Author? author, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
    {
        if (id == default)
            return ("", new InvalidInputError(id.Value));
        var key = id.Value;
        var output = GetOutput(key, optimizeFor);
        await Awaitable.BackgroundThreadAsync();
        Stream? stream;
        try
        {
            stream = await _client.GetAudioStreamAsync(id, pickStream, cancellationToken).ConfigureAwait(false);
        }
        catch (YoutubeExplodeException e)
        {
            return (output, new YouTubeExceptionError(e));
        }
        catch (PossiblyOutdatedYoutubeExplodeException e)
        {
            return (output, new PossiblyOutdatedVersionError(e));
        }

        if (stream == null)
            return (output, new StreamUnavailableError(id));
        var (ffmpeg, result) = await TranscodeAsync(stream, output, cancellationToken);
        if (ffmpeg == null)
            return result;
        using var disposeFFmpeg = ffmpeg;
        if (!await ffmpeg.WaitForExitAsync(cancellationToken).ConfigureAwait(false))
            return (output, SaveCacheError.Canceled);
        if (ffmpeg.HasExitedWithError)
            return (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!));
        await WriteMetadataAsync(id, output, title, author, cancellationToken);
        return (output, null);
    }

    /// <inheritdoc/>
    public override bool TryGetPath(VideoId source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (source != default)
            return base.TryGetPath(source, out cachedPath);
        cachedPath = null;
        return false;
    }

    public (string? VideoTitle, string? ChannelTitle, ChannelId? ChannelId) GetCachedMetadata(VideoId videoId)
    {
        if (videoId == default)
            return (null, null, null);
        var filePath = Path.Combine(Folder, videoId);
        var titleFile = $"{filePath}.{TitleExtension}";
        var authorFile = $"{filePath}.{AuthorExtension}";
        var videoTitle = File.Exists(titleFile) ? File.ReadAllText(titleFile).Trim() : null;
        return File.Exists(authorFile) && File.ReadAllLines(authorFile) is [var name, var id, ..]
            ? (videoTitle, name.Trim(), ChannelId.TryParse(id.Trim()))
            : (videoTitle, null, null);
    }

    private static async Awaitable<(FFmpegSL?, SaveCacheResult)> TranscodeAsync(Stream stream, string output, CancellationToken cancellationToken)
    {
        var ffmpeg = FFmpegSL.Start(Template with {Output = output});
        if (ffmpeg == null)
            return (null, (output, FFmpegSL.LastCaughtStartError));
        try
        {
            await stream.CopyToAsync(ffmpeg.Stdin.BaseStream, cancellationToken).ConfigureAwait(false);
            return (ffmpeg, default);
        }
        catch (Exception e)
        {
            ffmpeg.Dispose();
            return (null, (output, e));
        }
        finally
        {
            ffmpeg.Stdin.BaseStream.Close();
            await stream.DisposeAsync();
        }
    }

    private static async Awaitable WriteMetadataAsync(VideoId id, string output, string? title, Author? author, CancellationToken cancellationToken)
    {
        try
        {
            if (title != null)
                await File.WriteAllTextAsync(Path.ChangeExtension(output, TitleExtension), title, cancellationToken).ConfigureAwait(false);
            if (author != null)
                await File.WriteAllTextAsync(Path.ChangeExtension(output, AuthorExtension), $"{author.ChannelTitle}\n{author.ChannelId}", cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write metadata for the cached YouTube video {id}");
            Debug.LogException(e);
        }
    }

}
