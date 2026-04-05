using System;
using CommandSystem;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class StopCommand : ICommand
{

    public string Command => "stop";
    public string[] Aliases { get; } = ["end", "e"];
    public string Description => "Stops the YouTube player";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var result = PlaybackManager.Stop();
        response = result ? "Playback stopped." : "Playback has already stopped.";
        return result;
    }

}
