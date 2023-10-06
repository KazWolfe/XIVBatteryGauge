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
    [PluginService] public static IChatGui Chat { get; private set; }
    [PluginService] public static IClientState ClientState { get; private set; }
    [PluginService] public static ICommandManager CommandManager { get; private set; }
    [PluginService] public static ICondition Condition { get; private set; }
    [PluginService] public static IDataManager DataManager { get; private set; }
    [PluginService] public static IDtrBar DtrBar { get; private set; }
    [PluginService] public static IFramework Framework { get; private set; }
    [PluginService] public static IGameGui GameGui { get; private set; }
    [PluginService] public static IKeyState KeyState { get; private set; }
    [PluginService] public static ILibcFunction LibcFunction { get; private set; }
    [PluginService] public static IObjectTable Objects { get; private set; }
    [PluginService] public static ISigScanner SigScanner { get; private set; }
    [PluginService] public static ITargetManager TargetManager { get; private set; }
    [PluginService] public static IToastGui Toasts { get; private set; }
    [PluginService] public static IPluginLog PluginLog { get; private set; }
}