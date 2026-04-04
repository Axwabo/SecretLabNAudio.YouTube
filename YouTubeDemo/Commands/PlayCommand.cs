using System;
using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using YoutubeExplode.Videos;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class PlayCommand : ICommand
{

    public string Command => "play";
    public string[] Aliases { get; } = [];
    public string Description => "Plays a YouTube video";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0)
        {
            response = sender is PlayerCommandSender ? "Please provide an index, video ID or URL!" : "Please provide a video ID or URL!";
            return false;
        }

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

}
