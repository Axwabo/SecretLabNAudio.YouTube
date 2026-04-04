using System;
using LabApi.Features.Stores;
using LabApi.Loader.Features.Plugins;

namespace SecretLabNAudio.YouTube.Demo;

public sealed class YouTubeDemoPlugin : Plugin
{

    public override string Name => "SecretLabNAudio.YouTube.Demo";
    public override string Description => "Demo plugin for SecretLabNAudio.YouTube";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable() => CustomDataStoreManager.RegisterStore<SearchDataStore>();

    public override void Disable() => CustomDataStoreManager.UnregisterStore<SearchDataStore>();

}
