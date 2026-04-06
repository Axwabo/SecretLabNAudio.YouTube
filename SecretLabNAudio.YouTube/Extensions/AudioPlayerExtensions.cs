using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube.Extensions;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseYouTube(VideoId videoId) => player.Use(CreateYouTubeAudioProcessor.HighestQuality(videoId));

        public AudioPlayer UseCachedYouTube(VideoId videoId)
            => YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath)
                ? player.UseFile(cachedPath)
                : player.UseYouTube(videoId);

    }

}
