using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.YouTube.Caches;

/// <summary>
/// The error encountered when no adequate <see cref="IAudioStreamInfo"/> was chosen from the <see cref="StreamManifest"/>.
/// </summary>
/// <param name="VideoId">The ID of the video that was being processed.</param>
public sealed record StreamUnavailableError(VideoId VideoId) : SaveCacheError
{

    /// <summary>Converts the error to a human-readable error message.</summary>
    /// <returns>The error message.</returns>
    public override string ToString() => $"Failed to find a usable stream for video {VideoId}";

}

/// <summary>
/// The error encapsulating a <see cref="YoutubeExplodeException"/>.
/// </summary>
/// <param name="Exception">The exception that was thrown by YoutubeExplode.</param>
public sealed record YouTubeExceptionError(YoutubeExplodeException Exception) : SaveCacheError
{

    /// <summary>Converts the error to a human-readable error message.</summary>
    /// <returns>The error message.</returns>
    public override string ToString() => Exception.Message;

}

/// <summary>
/// An error encapsulating a <see cref="PossiblyOutdatedYoutubeExplodeException"/>, signifying that the YoutubeExplode installation might be outdated.
/// </summary>
/// <param name="Exception">The exception that was caught. It contains the inner exception.</param>
public sealed record PossiblyOutdatedYoutubeExplodeError(PossiblyOutdatedYoutubeExplodeException Exception) : SaveCacheError
{

    /// <summary>Converts the error to a human-readable error message.</summary>
    /// <returns>The error message.</returns>
    public override string ToString() => $"{Exception.Message} {Exception.InnerException.Message}";

}
