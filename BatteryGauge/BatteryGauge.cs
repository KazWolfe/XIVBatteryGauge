using System;
using System.Linq;
using BatteryGauge.Base;
using BatteryGauge.UI;
using BatteryGauge.UI.Windows;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BatteryGauge;

public class BatteryGauge : IDalamudPlugin {
    public DalamudPluginInterface PluginInterface { get; init; }
    public PluginConfig Configuration { get; init; }
    public WindowSystem WindowSystem { get; init; }

    private BatteryDtrBar BatteryDtrBar { get; init; }
    private readonly SettingsWindow _settingsWindow;

    public BatteryGauge(DalamudPluginInterface pluginInterface, IDtrBar dtrBar) {
        pluginInterface.Create<Injections>();
        
        this.PluginInterface = pluginInterface;
        this.Configuration = this.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
        this.Configuration.Initialize(this.PluginInterface);
        
        this.WindowSystem = new WindowSystem("BatteryGauge");
        this._settingsWindow = new SettingsWindow(this.Configuration);
        this.WindowSystem.AddWindow(this._settingsWindow);
        
        this.PluginInterface.UiBuilder.Draw += this.WindowSystem.Draw;
        this.PluginInterface.UiBuilder.OpenConfigUi += this.DrawConfigUI;
        
        this.BatteryDtrBar = new BatteryDtrBar(this.Configuration, dtrBar);
    }

    public void Dispose() {
        this.BatteryDtrBar.Dispose();
        this.WindowSystem.RemoveAllWindows();

        this.PluginInterface.UiBuilder.OpenConfigUi -= this.DrawConfigUI;
        this.PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;

        GC.SuppressFinalize(this);
    }

    private void DrawConfigUI() {
        this._settingsWindow.Toggle();
    }
}