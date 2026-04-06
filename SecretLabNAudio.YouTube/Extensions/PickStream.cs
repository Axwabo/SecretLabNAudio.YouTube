using System.Linq;

namespace SecretLabNAudio.YouTube.Extensions;

/// <summary>
/// A delegate that selects the most adequate stream info from the given manifest.
/// </summary>
/// <param name="manifest">The stream manifest to choose from.</param>
/// <returns>The most adequate audio stream info, or null if no stream was found.</returns>
public delegate IAudioStreamInfo? PickStream(StreamManifest manifest);

/// <summary>
/// Extension members for the <see cref="PickStream"/> delegate.
/// </summary>
public static class PickStreamExtensions
{

    private static readonly PickStream PickHighestBitrate = static manifest
        => manifest.GetAudioOnlyStreams().OrderByDescending(static e => e.Bitrate).FirstOrDefault()
           ?? manifest.GetAudioStreams().OrderByDescending(static e => e.Bitrate).FirstOrDefault();

    extension(PickStream)
    {

        /// <summary>
        /// Selects the highest bitrate audio-only stream, or falls back to the highest bitrate muxed stream.
        /// </summary>
        public static PickStream HighestBitrate => PickHighestBitrate;

    }

}
