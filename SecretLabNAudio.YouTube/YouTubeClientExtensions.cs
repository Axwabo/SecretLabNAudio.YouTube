using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

public static class YouTubeClientExtensions
{

    private static readonly YoutubeClient SharedClient = new();

    extension(YoutubeClient client)
    {

        public static YoutubeClient Shared => SharedClient;

        public async Task<Stream> GetAudioStreamAsync(VideoId id, CancellationToken cancellationToken = default)
        {
            var streams = await client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
            var best = streams.GetAudioOnlyStreams().OrderByDescending(static e => e.Bitrate).First();
            return await client.Videos.Streams.GetAsync(best, cancellationToken).ConfigureAwait(false);
        }

    }

}
