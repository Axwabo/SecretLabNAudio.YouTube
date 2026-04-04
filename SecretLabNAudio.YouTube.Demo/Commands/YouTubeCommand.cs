using System;
using CommandSystem;

namespace SecretLabNAudio.YouTube.Demo.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(ClientCommandHandler))]
public sealed class YouTubeCommand : ParentCommand
{

    public YouTubeCommand() => LoadGeneratedCommands();

    public override string Command => "YouTube";

    public override string[] Aliases { get; } = ["yt"];

    public override string Description => "YouTube playback commands";

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Please specify a valid subcommand!";
        return false;
    }

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new PlayCommand());
        RegisterCommand(new SearchCommand());
        RegisterCommand(new MyResultsCommand());
    }

}
