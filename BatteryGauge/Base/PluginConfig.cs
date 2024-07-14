using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace BatteryGauge.Base;

public enum ChargingDisplayMode {
    Hide           = 0b0000,
    TextOnly       = 0b0001,
    PercentageOnly = 0b0010,
    TextPercentage = 0b0011
}

public enum DischargingDisplayMode {
    Hide              = 0b0000,
    PercentageOnly    = 0b0001,
    RuntimeOnly       = 0b0010,
    PercentageRuntime = 0b0011
}


[Serializable]
public class PluginConfig : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public ChargingDisplayMode ChargingDisplayMode = ChargingDisplayMode.TextPercentage;
    public DischargingDisplayMode DischargingDisplayMode = DischargingDisplayMode.PercentageRuntime;

    public bool HideWhenFull = false;

    [NonSerialized]
    private IDalamudPluginInterface? _pluginInterface;

    public void Initialize(IDalamudPluginInterface @interface) {
        this._pluginInterface = @interface;
    }

    public void Save() {
        Injections.PluginLog.Debug("Saving config");
        this._pluginInterface!.SavePluginConfig(this);
    }
}