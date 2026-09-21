using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.YouTube.Caches;
using YoutubeExplode.Common;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extensions for the <see cref="YouTubeCache"/>.
/// </summary>
public static class CacheExtensions
{

    /// <param name="cache">The cache to use.</param>
    extension(YouTubeCache cache)
    {

        /// <summary>
        /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
        /// </summary>
        /// <param name="id">The ID of the video to download.</param>
        /// <param name="pickStream">A delegate that picks the most optimal stream from the manifest.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        public Awaitable<SaveCacheResult> CacheAsync(VideoId id, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(id, null, null, pickStream, optimizeFor, cancellationToken);

        /// <summary>
        /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
        /// </summary>
        /// <param name="id">The ID of the video to download.</param>
        /// <param name="title">The title of the video (if known).</param>
        /// <param name="author">The author of the video (if known).</param>
        /// <param name="pickStream">A delegate that picks the most optimal stream from the manifest.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        public async Awaitable<SaveCacheResult> CacheAsync(VideoId id, string? title, Author? author, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => await cache.CacheAsync(
                id,
                title,
                author,
                (client, videoId, token) => client.GetAudioStreamAsync(videoId, pickStream, token),
                optimizeFor,
                cancellationToken
            );

        /// <summary>
        /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
        /// </summary>
        /// <param name="video">The video to download.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        /// <remarks>The highest bitrate audio stream is used.</remarks>
        /// <seealso cref="PickStreamExtensions"/>
        public Awaitable<SaveCacheResult> CacheAsync(IVideo video, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video, PickStream.HighestBitrate, optimizeFor, cancellationToken);

        /// <summary>
        /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
        /// </summary>
        /// <param name="video">The video to download.</param>
        /// <param name="pickStream">A delegate that picks the most optimal stream from the manifest.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        public Awaitable<SaveCacheResult> CacheAsync(IVideo video, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video.Id, video.Title, video.Author, pickStream, optimizeFor, cancellationToken);

        /// <summary>
        /// Asynchronously starts and waits for FFmpeg to cache the given YouTube video.
        /// </summary>
        /// <param name="video">Metadata about the video.</param>
        /// <param name="streamInfo">The information to resolve the audio stream from.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        public Awaitable<SaveCacheResult> CacheAsync(IVideo video, IAudioStreamInfo streamInfo, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(
                video.Id,
                video.Title,
                video.Author,
                async (client, _, token) => await client.GetAudioStreamAsync(streamInfo, token),
                optimizeFor,
                cancellationToken
            );

        /// <summary>
        /// Asynchronously writes metadata to the disk about the video.
        /// </summary>
        /// <param name="video">Information about the video.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        public Awaitable WriteMetadataAsync(IVideo video, CancellationToken cancellationToken = default)
            => cache.WriteMetadataAsync(video.Id, video.Title, video.Author, cancellationToken);

    }

}
