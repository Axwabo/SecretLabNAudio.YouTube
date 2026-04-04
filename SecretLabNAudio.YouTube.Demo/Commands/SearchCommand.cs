using System;
using System.Linq;
using System.Threading;
using CommandSystem;
using LabApi.Features.Wrappers;
using UnityEngine;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class SearchCommand : ICommand
{

    private const string Dot = "•";

    public string Command => "search";
    public string[] Aliases { get; } = ["s"];
    public string Description => "Search for videos on YouTube";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0)
        {
            response = "Please provide a search query!";
            return false;
        }

        _ = SearchAsync(string.Join(" ", arguments), sender);
        response = "Searching, please wait...";
        return true;
    }

    private static async Awaitable SearchAsync(string query, ICommandSender sender)
    {
        var isPlayer = Player.TryGet(sender, out var player);
        var token = isPlayer
            ? player!.ReferenceHub.destroyCancellationToken
            : CancellationToken.None;
        var results = await Search.Top10Async(query, token);
        if (!isPlayer)
        {
            sender.Respond($"YouTube Search#Top 10 results:\n{string.Join("\n", results.Select(FormatSimple))}");
            return;
        }

        var store = player!.GetDataStore<ResultsDataStore>();
        store.Results.Clear();
        store.Results.AddRange(results);
    }

    private static string FormatSimple(VideoSearchResult result) => $"{result.Id}: {result.Title} {Dot} {result.Author} {Dot} {result.Duration}";

}
