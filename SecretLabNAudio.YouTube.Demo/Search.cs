using System.Collections.Generic;
using System.Threading;
using SecretLabNAudio.YouTube.Extensions;
using UnityEngine;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Demo;

public static class Search
{

    public static async Awaitable<IReadOnlyList<VideoSearchResult>> Top10Async(string query, CancellationToken cancellationToken)
        => await YoutubeClient.Shared.Search.GetVideosAsync(query, cancellationToken)
            .CollectAsync(10)
            .ConfigureAwait(false);

}
