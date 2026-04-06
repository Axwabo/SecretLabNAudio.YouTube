namespace SecretLabNAudio.YouTube.Exceptions;

public sealed class PossiblyOutdatedYoutubeExplodeException : Exception
{

    /// <summary>
    /// Gets the <see cref="Exception"/> instance that caused the current exception.
    /// This property cannot be null.
    /// </summary>
    public new Exception InnerException => base.InnerException!;

    public PossiblyOutdatedYoutubeExplodeException(Exception innerException) : base("YoutubeExplode might be outdated, check for updates.", innerException)
    {
    }

}
