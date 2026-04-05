using System;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class PlayCommand : ICommand, IUsageProvider
{

    public string Command => "play";
    public string[] Aliases { get; } = ["p"];
    public string Description => "Plays a YouTube video, or pauses/resumes the playback";
    public string[] Usage { get; } = ["[index/ID/URL]"];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission("yt.play"))
        {
            response = "You don't have permission to use this command.";
            return false;
        }

        if (arguments.Count == 0)
            return TogglePlaybackState(sender, out response);

        if (Player.TryGet(sender, out var player) && byte.TryParse(arguments.At(0), out var index))
        {
            var store = player.GetDataStore<SearchDataStore>();
            if (index == 0 || index >= store.Results.Count)
            {
                response = "Invalid index!";
                return false;
            }

            PlaybackManager.Play(store.Results[index - 1]);
            response = "Playback started.";
            return true;
        }

        var id = VideoId.TryParse(string.Join(" ", arguments));
        if (id == null)
        {
            response = "Invalid video ID!";
            return false;
        }

        PlaybackManager.Play(id.Value);
        response = "Playback started.";
        return true;
    }

    private static bool TogglePlaybackState(ICommandSender sender, out string response)
    {
        switch (PlaybackManager.TogglePause())
        {
            case null:
                response = sender is PlayerCommandSender ? "Please provide an index, video ID or URL!" : "Please provide a video ID or URL!";
                return false;
            case true:
                response = "Playback paused.";
                return true;
            case false:
                response = "Playback resumed.";
                return true;
        }
    }

}
