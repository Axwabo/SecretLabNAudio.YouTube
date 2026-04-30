using NAudio.Wave;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.YouTube;

public sealed record VideoIdPlaylistItem(VideoId Id) : PlaylistItem
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels)
        => StreamBasedFFmpegAudioProcessor.Create(CreateYouTubeAudioProcessor.GetHighestQualityAsync(Id), sampleRate, channels);

}

public sealed record YouTubeVideoPlaylistItem(IVideo Video) : PlaylistItem(Video.Title)
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels)
        => StreamBasedFFmpegAudioProcessor.Create(CreateYouTubeAudioProcessor.GetHighestQualityAsync(Video.Id), sampleRate, channels);

}
