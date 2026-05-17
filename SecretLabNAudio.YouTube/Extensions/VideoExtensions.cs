using YoutubeExplode.Channels;

namespace SecretLabNAudio.YouTube.Extensions;

public static class VideoExtensions
{

    extension(IVideo video)
    {

        public FullMetadata Metadata => (video.Title, video.Author.ChannelTitle, video.Author.ChannelId);

        public void Deconstruct(out string videoTitle, out string channelTitle, [NotNull] out ChannelId? channelId)
        {
            videoTitle = video.Title;
            channelTitle = video.Author.ChannelTitle;
            channelId = video.Author.ChannelId;
        }

    }

}
