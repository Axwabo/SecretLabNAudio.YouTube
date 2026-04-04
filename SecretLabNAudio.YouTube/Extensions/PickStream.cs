using System.Linq;
using YoutubeExplode.Videos.Streams;

namespace SecretLabNAudio.YouTube.Extensions;

public delegate IAudioStreamInfo? PickStream(StreamManifest manifest);

public static class PickStreamExtensions
{

    private static readonly PickStream PickHighestBitrate = static manifest
        => manifest.GetAudioOnlyStreams().OrderByDescending(static e => e.Bitrate).FirstOrDefault()
           ?? manifest.GetAudioStreams().OrderByDescending(static e => e.Bitrate).FirstOrDefault();

    extension(PickStream)
    {

        public static PickStream HighestBitrate => PickHighestBitrate;

    }

}
