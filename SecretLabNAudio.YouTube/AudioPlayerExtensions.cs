using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace SecretLabNAudio.YouTube;

public static class AudioPlayerExtensions
{

    private static readonly YoutubeClient Client = new();

    internal static async Task<Stream> GetStream(VideoId id, CancellationToken token)
    {
        var manifest = await Client.Videos.Streams.GetManifestAsync(id, token);
        var stream = manifest.GetAudioStreams()
            .OrderByDescending(static e => e is not IVideoStreamInfo)
            .ThenByDescending(static e => e.Bitrate)
            .First();
        return await Client.Videos.Streams.GetAsync(stream, token);
    }

    extension(AudioPlayer player)
    {

        public AudioPlayer UseYouTube(VideoId videoId)
            => player.Use(StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(token => GetStream(videoId, token)));

        public AudioPlayer UseYouTubeCached(VideoId videoId)
            => YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath)
                ? player.UseFile(cachedPath)
                : player.UseYouTube(videoId);

    }

}
