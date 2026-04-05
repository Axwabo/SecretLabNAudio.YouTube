using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.YouTube.Caches;
using YoutubeExplode.Search;

namespace SecretLabNAudio.YouTube.Extensions;

public static class CacheExtensions
{

    extension(YouTubeCache cache)
    {

        public Awaitable<SaveCacheResult> CacheAsync(VideoId id, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(id, null, null, pickStream, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(Video video, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video, PickStream.HighestBitrate, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(Video video, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video.Id, video.Title, video.Author, pickStream, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(VideoSearchResult result, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(result, PickStream.HighestBitrate, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(VideoSearchResult result, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(result.Id, result.Title, result.Author, pickStream, optimizeFor, cancellationToken);

    }

}
