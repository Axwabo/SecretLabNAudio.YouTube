namespace SecretLabNAudio.YouTube;

public interface IYouTubeVideoPlaylistItem : IVideoIdPlaylistItem
{

    IVideo Video { get; }

    VideoId IVideoIdPlaylistItem.Source => Video.Id;

}
