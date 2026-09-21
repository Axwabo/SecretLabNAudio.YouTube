using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg;
using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Extensions;
using static SecretLabNAudio.FFmpeg.Processors.AsyncFFmpegProcessorBase;
using static SecretLabNAudio.FFmpeg.Processors.StreamBasedFFmpegAudioProcessor;

namespace SecretLabNAudio.YouTube;

/// <summary>
/// Methods to create audio processors from YouTube videos.
/// </summary>
public static class CreateYouTubeAudioProcessor
{

    private static readonly FFmpegArguments Arguments = FFmpegArguments.PlayerCompatibleStdout with
    {
        Input = FFmpegArguments.StandardPipe,
        InputOptions = "-vn"
    };

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> that selects the highest bitrate audio stream from the given video.
    /// </summary>
    /// <param name="videoId">The ID of the video to play.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <include file='XmlDocs/Processor.xml' path='doc/returns'/>
    /// <include file='XmlDocs/Processor.xml' path='doc/remarks'/>
    /// <seealso cref="PickStreamExtensions"/>
    public static StreamBasedFFmpegAudioProcessor HighestQuality(VideoId videoId, double capacity = DefaultCapacity)
        => Create(GetHighestQualityAsync(videoId), Arguments, capacity);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> from the given video.
    /// </summary>
    /// <param name="videoId">The ID of the video to play.</param>
    /// <param name="pickStream">The delegate to select a stream from the manifest.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <include file='XmlDocs/Processor.xml' path='doc/returns'/>
    /// <include file='XmlDocs/Processor.xml' path='doc/remarks'/>
    public static StreamBasedFFmpegAudioProcessor ManualSelect(VideoId videoId, PickStream pickStream, double capacity = DefaultCapacity) => Create(async token =>
    {
        var stream = await YoutubeClient.Shared.GetAudioStreamAsync(videoId, pickStream, token);
        return stream ?? throw new NoStreamFoundException(videoId);
    }, Arguments, capacity);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> that selects the highest bitrate audio stream from the given video.
    /// </summary>
    /// <param name="videoId">The ID of the video to play.</param>
    /// <param name="getStreamTimeout">The maximum amount of time the stream resolving can take.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="client">The client to use. Set to null to use the shared client from <see cref="YouTubeClientExtensions"/>.</param>
    /// <include file='XmlDocs/Processor.xml' path='doc/returns'/>
    /// <include file='XmlDocs/Processor.xml' path='doc/remarks'/>
    /// <seealso cref="PickStreamExtensions"/>
    public static StreamBasedFFmpegAudioProcessor HighestQuality(VideoId videoId, TimeSpan getStreamTimeout, double capacity = DefaultCapacity, YoutubeClient? client = null)
        => ManualSelect(videoId, PickStream.HighestBitrate, getStreamTimeout, capacity, client);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> from the given video.
    /// </summary>
    /// <param name="videoId">The ID of the video to play.</param>
    /// <param name="pickStream">The delegate to select a stream from the manifest.</param>
    /// <param name="getStreamTimeout">The maximum amount of time the stream resolving can take.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="client">The client to use. Set to null to use the shared client from <see cref="YouTubeClientExtensions"/>.</param>
    /// <include file='XmlDocs/Processor.xml' path='doc/returns'/>
    /// <include file='XmlDocs/Processor.xml' path='doc/remarks'/>
    public static StreamBasedFFmpegAudioProcessor ManualSelect(VideoId videoId, PickStream pickStream, TimeSpan getStreamTimeout, double capacity = DefaultCapacity, YoutubeClient? client = null)
        => Create(async token =>
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(getStreamTimeout);
            try
            {
                var stream = await (client ?? YoutubeClient.Shared).GetAudioStreamAsync(videoId, pickStream, cts.Token);
                return stream ?? throw new NoStreamFoundException(videoId);
            }
            catch (OperationCanceledException e) when (!token.IsCancellationRequested)
            {
                throw new TimeoutException("Obtaining an audio stream timed out.", e);
            }
        }, Arguments, capacity);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> based on an <see cref="IAudioStreamInfo"/> object.
    /// </summary>
    /// <param name="info">The information to resolve the audio stream from.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="client">The client to use. Set to null to use the shared client from <see cref="YouTubeClientExtensions"/>.</param>
    /// <include file='XmlDocs/Processor.xml' path='doc/returns'/>
    /// <include file='XmlDocs/Processor.xml' path='doc/remarks'/>
    public static StreamBasedFFmpegAudioProcessor FromStreamInfo(IAudioStreamInfo info, double capacity = DefaultCapacity, YoutubeClient? client = null)
        => Create(async token => await (client ?? YoutubeClient.Shared).GetAudioStreamAsync(info, token), Arguments, capacity);

    /// <summary>
    /// Gets a delegate that resolves the highest quality audio stream for the given video.
    /// </summary>
    /// <param name="videoId">The ID of the video to get the stream for.</param>
    /// <returns>A delegate that asynchronously resolves the stream.</returns>
    public static Func<CancellationToken, Task<Stream>> GetHighestQualityAsync(VideoId videoId) => async token =>
    {
        var stream = await YoutubeClient.Shared.GetHighestQualityAudioStreamAsync(videoId, token);
        return stream ?? throw new NoStreamFoundException(videoId);
    };

}
