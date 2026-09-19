namespace SecretLabNAudio.YouTube.Exceptions;

/// <summary>
/// An exception wrapping a different <see cref="Exception"/> instance,
/// signaling that the error was most likely caused by an outdated YoutubeExplode installation.
/// </summary>
public sealed class PossiblyOutdatedYoutubeExplodeException : Exception
{

    /// <summary>
    /// Gets the <see cref="Exception"/> instance that caused the current exception.
    /// This property cannot be null.
    /// </summary>
    public new Exception InnerException => base.InnerException!;

    /// <summary>
    /// Initializes a new <see cref="PossiblyOutdatedYoutubeExplodeException"/> instance.
    /// </summary>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PossiblyOutdatedYoutubeExplodeException(Exception innerException) : base("YoutubeExplode might be outdated, check for updates.", innerException)
    {
    }

}
