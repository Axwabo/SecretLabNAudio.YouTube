using SecretLabNAudio.Core.Processors.Playlists;

namespace SecretLabNAudio.YouTube;

/// <summary>
/// Represents a <see cref="PlaylistItem"/> that is known to have an <see cref="IVideo"/> associated with it.
/// </summary>
public interface IYouTubeVideoPlaylistItem : IVideoIdPlaylistItem
{

    /// <summary>
    /// Metadata about the YouTube video.
    /// </summary>
    IVideo Video { get; }

    VideoId IVideoIdPlaylistItem.Source => Video.Id;

}
