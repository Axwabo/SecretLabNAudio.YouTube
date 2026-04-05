using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.YouTube.Caches;

public sealed record StreamUnavailableError(VideoId VideoId) : SaveCacheError
{

    public override string ToString() => $"Failed to find a usable stream for video {VideoId}";

}

public sealed record YouTubeExceptionError(Exception Exception) : SaveCacheError
{

    public override string ToString() => Exception.Message;

}
