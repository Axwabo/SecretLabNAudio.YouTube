using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// Extensions for the <see cref="AudioPlayer"/> class.
/// </summary>
public static class AudioPlayerExtensions
{

    /// <param name="player">The player to modify.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with an asynchronous processor that reads the highest-quality audio stream of the given YouTube video.
        /// </summary>
        /// <param name="videoId">The ID of the video to play.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer UseYouTube(VideoId videoId) => player.Use(CreateYouTubeAudioProcessor.HighestQuality(videoId));

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with an asynchronous processor that reads the highest-quality audio stream of the given YouTube video.
        /// </summary>
        /// <param name="video">The video to play.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer UseYouTube(IVideo video) => player.UseYouTube(video.Id);

        /// <summary>
        /// Replaces the <see cref="AudioPlayer.SampleProvider"/> with an asynchronous processor that reads the YouTube stream specified by an <see cref="IAudioStreamInfo"/>.
        /// </summary>
        /// <param name="streamInfo">The information to resolve the audio stream from.</param>
        /// <returns>The player itself.</returns>
        public AudioPlayer UseYouTube(IAudioStreamInfo streamInfo) => player.Use(CreateYouTubeAudioProcessor.FromStreamInfo(streamInfo));

        public AudioPlayer UseCachedYouTube(VideoId videoId)
            => YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath)
                ? player.UseFile(cachedPath)
                : player.UseYouTube(videoId);

        public AudioPlayer UseCachedYouTube(IVideo video) => player.UseCachedYouTube(video.Id);

    }

}
