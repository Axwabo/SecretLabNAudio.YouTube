using System.Threading.Tasks;
using SecretLabNAudio.YouTube.Exceptions;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extension members for the <see cref="YoutubeClient"/> class.
/// </summary>
public static class YouTubeClientExtensions
{

    private static readonly YoutubeClient SharedClient = new();

    extension(YoutubeClient client)
    {

        /// <summary>
        /// A shared <see cref="YoutubeClient"/> instance.
        /// </summary>
        public static YoutubeClient Shared => SharedClient;

        /// <summary>
        /// Gets the audio stream with the highest bitrate from a video.
        /// </summary>
        /// <param name="id">The ID of the video to get the stream of.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <include file='../XmlDocs/GetStream.xml' path='doc/returns'/>
        /// <include file='../XmlDocs/GetStream.xml' path='doc/exception'/>
        public Task<Stream?> GetHighestQualityAudioStreamAsync(VideoId id, CancellationToken cancellationToken = default)
            => client.GetAudioStreamAsync(id, PickStream.HighestBitrate, cancellationToken);

        /// <summary>
        /// Gets the most adequate audio stream from a video, given a <see cref="PickStream"/> delegate.
        /// </summary>
        /// <param name="id">The ID of the video to get the stream of.</param>
        /// <param name="pick">The delegate to select a stream from the manifest.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <include file='../XmlDocs/GetStream.xml' path='doc/returns'/>
        /// <include file='../XmlDocs/GetStream.xml' path='doc/exception'/>
        public async Task<Stream?> GetAudioStreamAsync(VideoId id, PickStream pick, CancellationToken cancellationToken = default)
        {
            if (id == default)
                return null;
            try
            {
                var streams = await client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
                var best = pick(streams);
                return best == null
                    ? null
                    : await client.Videos.Streams.GetAsync(best, cancellationToken).ConfigureAwait(false);
            }
            catch (YoutubeExplodeException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PossiblyOutdatedYoutubeExplodeException(e);
            }
        }

    }

}
