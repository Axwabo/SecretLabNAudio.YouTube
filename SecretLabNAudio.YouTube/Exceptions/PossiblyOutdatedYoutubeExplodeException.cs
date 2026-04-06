namespace SecretLabNAudio.YouTube.Exceptions;

public sealed class PossiblyOutdatedYoutubeExplodeException : Exception
{

    public new Exception InnerException => base.InnerException!;

    public PossiblyOutdatedYoutubeExplodeException(Exception innerException) : base("YoutubeExplode might be outdated, check for updates.", innerException)
    {
    }

}
