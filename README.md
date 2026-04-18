# SecretLabNAudio.YouTube

This module extends [SecretLabNAudio](https://github.com/Axwabo/SecretLabNAudio) to provide YouTube-based utilities.

> [!NOTE]
> SecretLabNAudio is not sponsored nor endorsed by NAudio.
> SecretLabNAudio.YouTube is not sponsored nor endorsed by YouTube.

This library has a number of open-source dependencies. See [Attributions](ATTRIBUTIONS.md)

# Installation

1. Install [SecretLabNAudio.FFmpeg](https://github.com/Axwabo/SecretLabNAudio#installation) version 2.0.0 or higher
   - Choose the single-file installation, or install the `FFmpeg` module
2. Download the `SecretLabNAudio.YouTube.dll` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
3. Download the `dependencies.zip` file from the releases page
4. Extract the DLLs from the `bin/` folder of the `dependencies.zip` archive to the **dependencies** directory
   - Linux: `~/.config/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
5. Place the `SecretLabNAudio.YouTube.dll` file into the **plugins** directory
   - Linux: `~/.config/SCP Secret Laboratory/LabAPI/plugins/<port>/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/<port>/`
6. Restart the server

> [!NOTE]
> If you would like to use the [demo plugin](https://github.com/Axwabo/SecretLabNAudio.YouTube/tree/main/YouTubeDemo)
> as well, download the `SecretLabNAudio.YouTube.Demo.dll` file and place it in the **plugins** directory.

| Plugins                                                             | Dependencies                                                                                |
|---------------------------------------------------------------------|---------------------------------------------------------------------------------------------|
| `SecretLabNAudio.YouTube` `SecretLabNAudio.YouTube.Demo` (optional) | `AngleSharp` `System.Text.Encoding.CodePages`  `System.Text.Encodings.Web` `YoutubeExplode` |

## Development

Simply install the `SecretLabNAudio.YouTube` package from NuGet.
It will automatically install dependencies in the project.

# Usage

To simply play a YouTube video, call the `UseYouTube` extension method.

<details>
<summary>Example</summary>

```csharp
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.YouTube.Extensions;

AudioPlayerPool.RentGloballyAudible()
    .WithVolume(0.5f)
    .UseYouTube("https://www.youtube.com/watch?v=dQw4w9WgXcQ") // implicit conversion
    .PoolOnEnd();
```

</details>

> [!TIP]
> See also: [demo project](https://github.com/Axwabo/SecretLabNAudio.YouTube/tree/main/YouTubeDemo)

If you want more control over the audio processor, call a method from the  `CreateYouTubeProcessor` class.

## Caching

If playing the same video multiple times, it's not ideal to wait for several seconds
for YoutubeExplode to be able to resolve the stream.

The `YouTubeCache` class provides methods to asynchronously cache audio from YouTube videos.

<details>
<summary>Example</summary>

Cache the videos you want:

```csharp
using SecretLabNAudio.FFmpeg.Caches;
using SecretLabNAudio.FFmpeg.Extensions;
using SecretLabNAudio.YouTube.Caches;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

// plugin class
public override void Enable()
{
    _ = CacheOrLogAsync("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
    _ = CacheOrLogAsync("https://youtu.be/eR8VEqaaWY8"); // short links work, too
    _ = CacheOrLogAsync("WX9miGKXyrw"); // video ID only
}

private static async Awaitable CacheOrLogAsync(string videoIdOrUrl)
{
    var (_, error) = await YouTubeCache.Shared.CacheIfNotCachedAsync(videoIdOrUrl, OptimizeFor.FileSize);
    if (error is not null)
        Logger.Error($"Failed caching {videoIdOrUrl}: {error.ToHumanReadableString()}");
}
```

Play a cached video (fallback to YouTube processor if not cached):

```csharp
using SecretLabNAudio.YouTube.Extensions;

const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

audioPlayer.UseCachedYouTube(url);
```

The above code basically does this:

```csharp
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.YouTube.Caches;
using SecretLabNAudio.YouTube.Extensions;
using YoutubeExplode.Videos;

// throws on invalid ID
var videoId = VideoId.Parse("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

if (YouTubeCache.Shared.TryGetPath(videoId, out var cachedPath))
    audioPlayer.UseFile(cachedPath);
else
    audioPlayer.UseYouTube(videoId);
```

</details>

