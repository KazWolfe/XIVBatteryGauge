using BatteryGauge.Base;
using BatteryGauge.UI;
using BatteryGauge.UI.Windows;
using Dalamud.Game;
using Dalamud.Plugin;

namespace BatteryGauge;

public class BatteryGauge : IDalamudPlugin {
    public static BatteryGauge Instance = null!;
    
    public string Name => "BatteryGauge";
    
    public DalamudPluginInterface PluginInterface { get; init; }
    public PluginConfig Configuration { get; init; }
    public PatchedWindowSystem WindowSystem { get; init; }

    private BatteryDtrBar BatteryDtrBar { get; init; }
    

    public BatteryGauge(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Injections>();

        Instance = this;

        this.PluginInterface = pluginInterface;
        this.Configuration = this.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
        this.Configuration.Initialize(this.PluginInterface);

        this.WindowSystem = new PatchedWindowSystem(this.Name);
        this.PluginInterface.UiBuilder.Draw += this.WindowSystem.Draw;
        this.PluginInterface.UiBuilder.OpenConfigUi += this.DrawConfigUI;

        this.BatteryDtrBar = new BatteryDtrBar();
    }

    public void Dispose() {
        this.BatteryDtrBar.Dispose();
        this.WindowSystem.RemoveAllWindows();

        this.PluginInterface.UiBuilder.OpenConfigUi -= this.DrawConfigUI;
        this.PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;
    }

    private void DrawConfigUI() {
        var instance = this.WindowSystem.GetWindow(SettingsWindow.WindowKey);
        
        if (instance == null) {
            this.WindowSystem.AddWindow(new SettingsWindow());
        }
    }
}