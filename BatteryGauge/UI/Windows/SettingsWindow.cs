using BatteryGauge.Battery;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BatteryGauge.UI.Windows; 

public class SettingsWindow : Window {
    public const string WindowKey = "Settings";
    
    private readonly BatteryGauge _plugin = BatteryGauge.Instance;

    public SettingsWindow() : base(WindowKey, ImGuiWindowFlags.None, true) {
        this.IsOpen = true;
    }
    
    public override void OnClose() {
        this._plugin.WindowSystem.RemoveWindow(this);
    }
    
    public override void Draw() {
        ImGui.Text("Hello, world!");
        ImGui.Text($"Battery Percentage: {SystemPower.ChargePercentage}%");
        ImGui.Text($"Limetime (sec): {SystemPower.LifetimeSeconds}");
        ImGui.Text($"Charging: {SystemPower.IsCharging}");
        ImGui.Text($"Has Battery: {SystemPower.HasBattery}");
    }
}