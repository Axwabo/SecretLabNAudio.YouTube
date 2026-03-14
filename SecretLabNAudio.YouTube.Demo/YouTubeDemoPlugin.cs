using System;
using LabApi.Loader.Features.Plugins;

namespace SecretLabNAudio.YouTube.Demo;

public sealed class YouTubeDemoPlugin : Plugin
{

    public override string Name { get; }
    public override string Description { get; }
    public override string Author { get; }
    public override Version Version { get; }
    public override Version RequiredApiVersion { get; }

    public override void Enable() => throw new NotImplementedException();

    public override void Disable() => throw new NotImplementedException();

}
