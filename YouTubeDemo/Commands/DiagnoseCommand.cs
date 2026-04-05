using System;
using CommandSystem;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class DiagnoseCommand : ICommand
{

    public string Command => "diagnose";
    public string[] Aliases { get; } = ["diag"];
    public string Description => "Diagnose YouTube playback";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var error = PlaybackManager.Error;
        response = error ?? "Playback is functional";
        return error == null;
    }

}
