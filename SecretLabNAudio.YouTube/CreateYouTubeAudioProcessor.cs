using SecretLabNAudio.FFmpeg.Processors;
using SecretLabNAudio.YouTube.Extensions;

namespace SecretLabNAudio.YouTube;

public static class CreateYouTubeAudioProcessor
{

    public static StreamBasedFFmpegAudioProcessor HighestQuality(VideoId videoId) => StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(async token =>
    {
        var stream = await YoutubeClient.Shared.GetHighestQualityAudioStreamAsync(videoId, token);
        return stream ?? throw new NoStreamFoundException(videoId);
    });

    public static StreamBasedFFmpegAudioProcessor ManualSelect(VideoId videoId, PickStream pickStream) => StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(async token =>
    {
        var stream = await YoutubeClient.Shared.GetAudioStreamAsync(videoId, pickStream, token);
        return stream ?? throw new NoStreamFoundException(videoId);
    });

    public static StreamBasedFFmpegAudioProcessor HighestQuality(VideoId videoId, TimeSpan getStreamTimeout)
        => ManualSelect(videoId, PickStream.HighestBitrate, getStreamTimeout);

    public static StreamBasedFFmpegAudioProcessor ManualSelect(VideoId videoId, PickStream pickStream, TimeSpan getStreamTimeout) => StreamBasedFFmpegAudioProcessor.CreatePlayerCompatible(async token =>
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(getStreamTimeout);
        var stream = await YoutubeClient.Shared.GetAudioStreamAsync(videoId, pickStream, cts.Token);
        return stream ?? throw new NoStreamFoundException(videoId);
    });

}
