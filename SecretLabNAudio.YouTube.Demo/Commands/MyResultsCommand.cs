using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class MyResultsCommand : ICommand
{

    public string Command => "myResults";
    public string[] Aliases { get; } = [];
    public string Description => "Shows your results (players only)";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Player.TryGet(sender, out var player))
        {
            response = "Only players can use this command.";
            return false;
        }

        var store = player.GetDataStore<SearchDataStore>();
        if (store.Results.Count == 0)
        {
            response = store.IsSearching ? "Searching, no results yet." : "No results yet.";
            return false;
        }

        response = $"Results:\n{string.Join("\n", store.Results.Select(SearchCommand.FormatResponse))}{(store.IsSearching ? "\nSearching in progress..." : "")}";
        return true;
    }

}
