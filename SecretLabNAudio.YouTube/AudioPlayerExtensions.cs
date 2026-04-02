using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseYouTube(VideoId videoId)
            => player.Use(StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(token => YoutubeClient.Shared.GetAudioStreamAsync(videoId, token)));

        public AudioPlayer UseCachedYouTube(VideoId videoId)
            => YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath)
                ? player.UseFile(cachedPath)
                : player.UseYouTube(videoId);

    }

}
