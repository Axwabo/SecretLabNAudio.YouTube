using SecretLabNAudio.YouTube.Caches;
using YoutubeExplode.Channels;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extensions for playlist items.
/// </summary>
public static class PlaylistItemExtensions
{

    extension(IVideoIdPlaylistItem item)
    {

        /// <summary>
        /// Attempts to get known or cached metadata based on the playlist item.
        /// </summary>
        /// <param name="videoTitle">The title of the video.</param>
        /// <param name="channelTitle">The title of the author's channel.</param>
        /// <param name="channelId">The ID of the author.</param>
        /// <returns>Whether <b>all</b> properties could be retrieved.</returns>
        public bool TryGetFullMetadata([NotNullWhen(true)] out string? videoTitle, [NotNullWhen(true)] out string? channelTitle, [NotNullWhen(true)] out ChannelId? channelId)
        {
            if (item is IYouTubeVideoPlaylistItem {Video: var video})
            {
                (videoTitle, channelTitle, channelId) = video;
                return true;
            }

            (videoTitle, channelTitle, channelId) = YouTubeCache.Shared.GetCachedMetadata(item.Source);
            return videoTitle != null && channelTitle != null && channelId != null;
        }

        /// <summary>
        /// Gets the known or cached metadata based on the playlist item, or asynchronously retrieves it.
        /// </summary>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        /// <remarks>The method will complete synchronously if the metadata is already known or cached.</remarks>
        public async Awaitable<FullMetadata> GetMetadataAsync()
        {
            if (item.TryGetFullMetadata(out var videoTitle, out var channelTitle, out var channelId))
                return (videoTitle, channelTitle, channelId.Value);
            await Awaitable.BackgroundThreadAsync();
            var resolvedVideo = await YoutubeClient.Shared.Videos.GetAsync(item.Source).ConfigureAwait(false);
            return resolvedVideo.Metadata;
        }

        /// <summary>
        /// Gets the cached metadata based on the playlist item, or caches the result (asynchronously retrieving metadata if needed).
        /// </summary>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
        /// <remarks>The method will complete synchronously if the metadata is already cached.</remarks>
        public async Awaitable<FullMetadata> GetAndWriteMetadataAsync()
        {
            var (videoTitle, channelTitle, channelId) = YouTubeCache.Shared.GetCachedMetadata(item.Source);
            if (videoTitle != null && channelTitle != null && channelId != null)
                return (videoTitle, channelTitle, channelId.Value);
            if (item is IYouTubeVideoPlaylistItem {Video: var video})
            {
                await YouTubeCache.Shared.WriteMetadataAsync(video);
                return video.Metadata;
            }

            await Awaitable.BackgroundThreadAsync();
            var resolvedVideo = await YoutubeClient.Shared.Videos.GetAsync(item.Source);
            await YouTubeCache.Shared.WriteMetadataAsync(resolvedVideo);
            return resolvedVideo.Metadata;
        }

    }

}
