using SecretLabNAudio.YouTube.Caches;
using YoutubeExplode.Channels;

namespace SecretLabNAudio.YouTube.Extensions;

public static class PlaylistItemExtensions
{

    extension(IVideoIdPlaylistItem item)
    {

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

        public async Awaitable<FullMetadata> GetMetadataAsync()
        {
            if (item.TryGetFullMetadata(out var videoTitle, out var channelTitle, out var channelId))
                return (videoTitle, channelTitle, channelId.Value);
            await Awaitable.BackgroundThreadAsync();
            var resolvedVideo = await YoutubeClient.Shared.Videos.GetAsync(item.Source);
            return resolvedVideo.Metadata;
        }

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
