using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Caches;
using YoutubeExplode.Exceptions;

namespace SecretLabNAudio.YouTube.Extensions;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseYouTube(VideoId videoId)
            => player.Use(StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(async token =>
            {
                var stream = await YoutubeClient.Shared.GetHighestQualityAudioStreamAsync(videoId, token);
                return stream ?? throw new VideoUnavailableException($"No stream found for video {videoId}");
            }));

        public AudioPlayer UseCachedYouTube(VideoId videoId)
            => YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath)
                ? player.UseFile(cachedPath)
                : player.UseYouTube(videoId);

    }

}
