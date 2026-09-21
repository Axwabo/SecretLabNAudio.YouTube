using YoutubeExplode.Channels;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extensions for <see cref="IVideo"/>s.
/// </summary>
public static class VideoExtensions
{

    extension(IVideo video)
    {

        /// <summary>
        /// Gets metadata as a tuple.
        /// </summary>
        public FullMetadata Metadata => (video.Title, video.Author.ChannelTitle, video.Author.ChannelId);

        /// <summary>
        /// Deconstructs the video into metadata variables.
        /// </summary>
        /// <param name="videoTitle">The title of the video.</param>
        /// <param name="channelTitle">The title of the author's channel.</param>
        /// <param name="channelId">The ID of the author. The value is not null.</param>
        public void Deconstruct(out string videoTitle, out string channelTitle, [NotNull] out ChannelId? channelId)
        {
            videoTitle = video.Title;
            channelTitle = video.Author.ChannelTitle;
            channelId = video.Author.ChannelId;
        }

    }

}
