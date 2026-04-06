namespace SecretLabNAudio.YouTube.Exceptions;

public sealed class NoStreamFoundException : YoutubeExplodeException
{

    public NoStreamFoundException(VideoId id) : base($"No stream found for video {id}")
    {
    }

}
