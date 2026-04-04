using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandSystem;
using LabApi.Features.Wrappers;
using SecretLabNAudio.YouTube.Extensions;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Demo.Commands;

public sealed class SearchCommand : ICommand
{

    private const string Dot = "•";
    private const int Max = 10;

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

        var query = string.Join(" ", arguments);
        if (!Player.TryGet(sender, out var player))
        {
            _ = RespondSimpleAsync(query, sender);
            response = "Searching, please wait...";
            return true;
        }

        var store = player.GetDataStore<SearchDataStore>();
        if (store.IsSearching)
        {
            response = "Already searching!";
            return false;
        }

        _ = SearchAsync(query, sender, store);
        response = "Searching, please wait...";
        return true;
    }

    private static async Awaitable RespondSimpleAsync(string query, ICommandSender sender)
    {
        await Awaitable.NextFrameAsync();
        sender.Respond($"Video ID#Title {Dot} Author {Dot} Duration");
        var count = 0;
        await foreach (var result in YoutubeClient.Shared.Search.GetVideosAsync(query).ConfigureAwait(false))
        {
            sender.Respond($"{result.Id}#{result.Title} {Dot} {result.Author} {Dot} {result.Duration}");
            if (++count >= Max)
                break;
        }
    }

    private static async Awaitable SearchAsync(string query, ICommandSender sender, SearchDataStore store)
    {
        store.IsSearching = true;
        store.Results.Clear();
        try
        {
            await Awaitable.NextFrameAsync();
            sender.Respond($"YouTube Search##Index {Dot} Title {Dot} Author {Dot} Duration");
            await AppendAndRespondAsync(query, sender, store.Results, store.Owner.ReferenceHub.destroyCancellationToken);
        }
        catch (Exception e)
        {
            sender.Respond($"YouTube Search#{e}", false);
        }
        finally
        {
            store.IsSearching = false;
        }
    }

    private static async Awaitable AppendAndRespondAsync(string query, ICommandSender sender, List<VideoSearchResult> results, CancellationToken cancellationToken)
    {
        var count = 0;
        var enumerable = YoutubeClient.Shared.Search.GetVideosAsync(query, cancellationToken).ConfigureAwait(false);
        await foreach (var result in enumerable)
        {
            results.Add(result);
            sender.Respond($"YouTube Search##{count + 1} {Dot} {result.Title} {Dot} {result.Author} {Dot} {result.Title}");
            if (++count >= Max)
                break;
        }
    }

}
