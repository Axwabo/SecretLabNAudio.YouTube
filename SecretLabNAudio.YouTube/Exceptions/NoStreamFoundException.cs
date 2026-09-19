namespace SecretLabNAudio.YouTube.Exceptions;

/// <summary>
/// An exception indicating that no audio stream could be selected.
/// </summary>
public sealed class NoStreamFoundException : Exception
{

    /// <summary>
    /// Initializes a new <see cref="NoStreamFoundException"/> instance.
    /// </summary>
    /// <param name="id">The ID of the video being processed.</param>
    public NoStreamFoundException(VideoId id) : base($"No stream found for video {id}")
    {
    }

}
