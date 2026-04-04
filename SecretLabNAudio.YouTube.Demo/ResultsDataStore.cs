using System.Collections.Generic;
using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Demo;

public sealed class ResultsDataStore : CustomDataStore<ResultsDataStore>
{

    public ResultsDataStore(Player owner) : base(owner)
    {
    }

    public List<VideoSearchResult> Results { get; } = [];

}
