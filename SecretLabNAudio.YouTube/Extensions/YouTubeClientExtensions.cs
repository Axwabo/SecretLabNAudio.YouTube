using System.Linq;
using System.Threading.Tasks;

namespace SecretLabNAudio.YouTube.Extensions;

public static class YouTubeClientExtensions
{

    private static readonly YoutubeClient SharedClient = new();

    extension(YoutubeClient client)
    {

        public static YoutubeClient Shared => SharedClient;

        public async Task<Stream?> GetAudioStreamAsync(VideoId id, CancellationToken cancellationToken = default)
        {
            if (id == default)
                return null;
            var streams = await client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
            // TODO: user choice
            var best = streams.GetAudioOnlyStreams()
                .OrderByDescending(static e => e.Bitrate)
                .FirstOrDefault();
            return best == null
                ? null
                : await client.Videos.Streams.GetAsync(best, cancellationToken).ConfigureAwait(false);
        }

    }

}
