using System.Diagnostics.CodeAnalysis;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BatteryGauge.Base;


// disable nullable warnings as all of these are injected. if they're missing, we have serious issues.
#pragma warning disable CS8618

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public class Injections {
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] public static IPluginLog PluginLog { get; private set; }
}