using System;
using CommandSystem;
using LabApi.Features.Permissions;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class StopCommand : ICommand
{

    public string Command => "stop";
    public string[] Aliases { get; } = ["end", "e"];
    public string Description => "Stops the YouTube player";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission("yt.play"))
        {
            response = "You don't have permission to use this command.";
            return false;
        }

        var result = PlaybackManager.Stop();
        response = result ? "Playback stopped." : "Playback has already stopped.";
        return result;
    }

}
