using System.Collections.Generic;
using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Demo;

public sealed class SearchDataStore : CustomDataStore<SearchDataStore>
{

    public SearchDataStore(Player owner) : base(owner)
    {
    }

    public bool IsSearching { get; set; }

    public List<VideoSearchResult> Results { get; } = [];

}
