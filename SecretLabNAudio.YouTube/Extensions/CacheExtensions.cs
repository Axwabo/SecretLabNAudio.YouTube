using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube.Extensions;

public static class CacheExtensions
{

    extension(YouTubeCache cache)
    {

        public Awaitable<SaveCacheResult> CacheAsync(VideoId id, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(id, null, null, pickStream, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(IVideo video, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video, PickStream.HighestBitrate, optimizeFor, cancellationToken);

        public Awaitable<SaveCacheResult> CacheAsync(IVideo video, PickStream pickStream, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video.Id, video.Title, video.Author, pickStream, optimizeFor, cancellationToken);

        public Awaitable WriteMetadataAsync(IVideo video, CancellationToken cancellationToken = default)
            => cache.WriteMetadataAsync(video.Id, video.Title, video.Author, cancellationToken);

    }

}
