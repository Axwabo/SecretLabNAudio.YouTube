using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.YouTube.Caches;

public sealed record StreamUnavailableError(VideoId VideoId) : SaveCacheError
{

    public override string ToString() => $"Failed to find a usable stream for video {VideoId}";

}

public sealed record YouTubeExceptionError(YoutubeExplodeException Exception) : SaveCacheError
{

    public override string ToString() => Exception.Message;

}

public sealed record PossiblyOutdatedVersionError(PossiblyOutdatedYoutubeExplodeException Exception) : SaveCacheError
{

    public override string ToString() => $"{Exception.Message} {Exception.InnerException.Message}";

}