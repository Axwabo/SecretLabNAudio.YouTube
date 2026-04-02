using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.YouTube.Caches;

namespace SecretLabNAudio.YouTube.Extensions;

public static class CacheExtensions
{

    extension(YouTubeCache cache)
    {

        public Awaitable<SaveCacheResult> CacheAsync(Video video, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAsync(video.Id, optimizeFor, cancellationToken);

    }

}
