using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace BatteryGauge.Base; 

[Serializable]
public class PluginConfig : IPluginConfiguration {
    public int Version { get; set; } = 0;

    [NonSerialized]
    private DalamudPluginInterface? _pluginInterface;

    public void Initialize(DalamudPluginInterface @interface) {
        this._pluginInterface = @interface;
    }

    public void Save() {
        this._pluginInterface!.SavePluginConfig(this);
    }
}