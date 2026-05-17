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

    }

}
