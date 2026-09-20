using System.Collections.Generic;
using System.Linq;
using SecretLabNAudio.Core.Processors.Playlists;

namespace SecretLabNAudio.YouTube.Extensions;

public static class PlaylistExtensions
{

    extension(LazyPlaylist playlist)
    {

        public LazyPlaylist AddYouTube(VideoId videoId) => playlist.Add(new VideoIdPlaylistItem(videoId));

        public LazyPlaylist AddYouTube(IVideo video) => playlist.Add(new YouTubeVideoPlaylistItem(video));

        public LazyPlaylist AddCachedYouTube(VideoId videoId) => playlist.Add(new CachedVideoIdPlaylistItem(videoId));

        public LazyPlaylist AddCachedYouTUbe(IVideo video) => playlist.Add(new CachedYouTubeVideoPlaylistItem(video));

        public VideoId? CurrentVideoId => (playlist.CurrentItem as IVideoIdPlaylistItem)?.Source;

        public IVideo? CurrentVideo => (playlist.CurrentItem as IYouTubeVideoPlaylistItem)?.Video;

        public IEnumerable<VideoId> RemainingVideoIds => playlist.Items
            .Skip(playlist.Index + 1)
            .OfType<IVideoIdPlaylistItem>()
            .Select(e => e.Source);

        public IEnumerable<IVideo> RemainingVideos => playlist.Items
            .Skip(playlist.Index + 1)
            .OfType<IYouTubeVideoPlaylistItem>()
            .Select(e => e.Video);

    }

}
