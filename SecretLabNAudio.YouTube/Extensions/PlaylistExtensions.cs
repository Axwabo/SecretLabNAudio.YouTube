using System.Collections.Generic;
using System.Linq;
using SecretLabNAudio.Core.Processors.Playlists;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extensions for the <see cref="LazyPlaylist"/> class.
/// </summary>
public static class PlaylistExtensions
{

    extension(LazyPlaylist playlist)
    {

        /// <summary>
        /// Adds a YouTube video to the end of the playlist.
        /// </summary>
        /// <param name="videoId">The ID of the video to enqueue.</param>
        /// <returns>The playlist itself.</returns>
        /// <remarks>The item won't be included when querying <see cref="GetRemainingVideos"/>.</remarks>
        /// <seealso cref="AddYouTube(LazyPlaylist,IVideo)"/>
        public LazyPlaylist AddYouTube(VideoId videoId) => playlist.Add(new VideoIdPlaylistItem(videoId));

        /// <summary>
        /// Adds a YouTube video to the end of the playlist.
        /// </summary>
        /// <param name="video">Metadata about the video to enqueue.</param>
        /// <returns>The playlist itself.</returns>
        /// <seealso cref="AddYouTube(LazyPlaylist,IVideo)"/>
        public LazyPlaylist AddYouTube(IVideo video) => playlist.Add(new YouTubeVideoPlaylistItem(video));

        /// <summary>
        /// Adds a cached YouTube video to the end of the playlist.
        /// </summary>
        /// <param name="videoId">The ID of the video to enqueue.</param>
        /// <returns>The playlist itself.</returns>
        /// <remarks>
        /// The item won't be included when querying <see cref="GetRemainingVideos"/>.
        /// If the file was cached in <see cref="P:SecretLabNAudio.YouTube.Caches.YouTubeCache.Shared">the shared cache</see>, the cached version will be used.
        /// </remarks>
        /// <seealso cref="AddCachedYouTube(LazyPlaylist,IVideo)"/>
        public LazyPlaylist AddCachedYouTube(VideoId videoId) => playlist.Add(new CachedVideoIdPlaylistItem(videoId));

        /// <summary>
        /// Adds a cached YouTube video to the end of the playlist.
        /// </summary>
        /// <param name="video">Metadata about the video to enqueue.</param>
        /// <returns>The playlist itself.</returns>
        /// <remarks>If the file was cached in <see cref="P:SecretLabNAudio.YouTube.Caches.YouTubeCache.Shared">the shared cache</see>, the cached version will be used.</remarks>
        public LazyPlaylist AddCachedYouTube(IVideo video) => playlist.Add(new CachedYouTubeVideoPlaylistItem(video));

        /// <summary>
        /// Gets the current item's <see cref="VideoId"/> if it's known.
        /// </summary>
        public VideoId? CurrentVideoId => (playlist.CurrentItem as IVideoIdPlaylistItem)?.Source;

        /// <summary>
        /// Gets the current item's <see cref="IVideo"/> if it's known.
        /// </summary>
        public IVideo? CurrentVideo => (playlist.CurrentItem as IYouTubeVideoPlaylistItem)?.Video;

        /// <summary>
        /// Gets the <see cref="VideoId"/>s of known remaining playlist items.
        /// Items that are not <see cref="IVideoIdPlaylistItem"/> are skipped.
        /// </summary>
        /// <param name="includeCurrent">Whether to include the current item.</param>
        /// <returns>An enumerable containing known <see cref="VideoId"/>s.</returns>
        public IEnumerable<VideoId> GetRemainingVideoIds(bool includeCurrent = true) => playlist.GetRemainingItems(includeCurrent)
            .OfType<IVideoIdPlaylistItem>()
            .Select(e => e.Source);

        /// <summary>
        /// Gets the <see cref="IVideo"/>s of known remaining playlist items.
        /// Items that are not <see cref="IYouTubeVideoPlaylistItem"/> are skipped.
        /// </summary>
        /// <param name="includeCurrent">Whether to include the current item.</param>
        /// <returns>An enumerable containing known <see cref="IVideo"/>s.</returns>
        public IEnumerable<IVideo> GetRemainingVideos(bool includeCurrent = true) => playlist.GetRemainingItems(includeCurrent)
            .Skip(playlist.Index + 1)
            .OfType<IYouTubeVideoPlaylistItem>()
            .Select(e => e.Video);

    }

}
