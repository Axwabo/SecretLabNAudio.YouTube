using System.Threading.Tasks;

namespace SecretLabNAudio.YouTube.Extensions;

public static class YouTubeClientExtensions
{

    private static readonly YoutubeClient SharedClient = new();

    extension(YoutubeClient client)
    {

        public static YoutubeClient Shared => SharedClient;

        public async Task<Stream?> GetAudioStreamAsync(VideoId id, PickStream pick, CancellationToken cancellationToken = default)
        {
            if (id == default)
                return null;
            var streams = await client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
            var best = pick(streams);
            return best == null
                ? null
                : await client.Videos.Streams.GetAsync(best, cancellationToken).ConfigureAwait(false);
        }

        public Task<Stream?> GetHighestQualityAudioStreamAsync(VideoId id, CancellationToken cancellationToken = default)
            => client.GetAudioStreamAsync(id, PickStream.HighestBitrate, cancellationToken);

    }

}
