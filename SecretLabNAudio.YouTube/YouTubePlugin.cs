using LabApi.Loader.Features.Plugins;

namespace SecretLabNAudio.YouTube;

internal sealed class YouTubePlugin : Plugin
{

    public override string Name => "SecretLabNAudio.YouTube";

    public override string Description => "YouTube integration for SecretLabNAudio";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable() => throw new NotImplementedException();

    public override void Disable() => throw new NotImplementedException();

}
