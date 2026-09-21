using SecretLabNAudio.Core.Processors.Playlists;

namespace SecretLabNAudio.YouTube;

/// <summary>
/// Represents a <see cref="PlaylistItem"/> that is known to have a <see cref="VideoId"/> associated with it.
/// </summary>
public interface IVideoIdPlaylistItem
{

    /// <summary>
    /// The ID of the YouTube video.
    /// </summary>
    VideoId Source { get; }

}
