using SecretLabNAudio.FFmpeg.Caches;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube;

public sealed record StreamUnavailableError(VideoId VideoId) : SaveCacheError
{

    public override string ToString() => $"Failed to find a usable stream for video {VideoId}";

}
