using System;
using System.Numerics;
using BatteryGauge.Base;
using BatteryGauge.Battery;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BatteryGauge.UI.Windows; 

public class SettingsWindow : Window {
    public const string WindowKey = "BatteryGauge Settings";
    
    private readonly BatteryGauge _plugin = BatteryGauge.Instance;
    private readonly PluginConfig _config = BatteryGauge.Instance.Configuration;

    public SettingsWindow() : base(WindowKey, ImGuiWindowFlags.None, true) {
        this.IsOpen = true;

        this.Size = new Vector2(400, 200);
        this.SizeCondition = ImGuiCond.Appearing;
    }

    public override void OnOpen() {

    }

    public override void OnClose() {
        this._plugin.WindowSystem.RemoveWindow(this);
    }
    
    public override void Draw() {
        /* Battery Warning */
        if (!SystemPower.HasBattery) {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.PushTextWrapPos();
            ImGui.Text("This device either does not have a battery, or the battery is in an error state!");
            ImGui.PopTextWrapPos();
            ImGui.PopStyleColor();
        }

        /* Selector Content */
        if (ImGui.BeginCombo("Charge Mode", Enum.GetName(this._config.ChargingDisplayMode))) {
            foreach (var mode in Enum.GetValues<ChargingDisplayMode>()) {
                var selected = (mode == this._config.ChargingDisplayMode);
                if (ImGui.Selectable(Enum.GetName(mode), selected)) {
                    this._config.ChargingDisplayMode = mode;
                    this._config.Save();
                }

                if (selected) {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.Checkbox("Hide When Battery Charged", ref this._config.HideWhenFull)) {
            this._config.Save();
        }

        ImGui.Dummy(new Vector2(0, 10));
        
        if (ImGui.BeginCombo("Discharge Mode", Enum.GetName(this._config.DischargingDisplayMode))) {
            foreach (var mode in Enum.GetValues<DischargingDisplayMode>()) {
                var selected = (mode == this._config.DischargingDisplayMode);
                if (ImGui.Selectable(Enum.GetName(mode), selected)) {
                    this._config.DischargingDisplayMode = mode;
                    this._config.Save();
                }

                if (selected) {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        
        /* Debug Data */
#if DEBUG
        ImGui.Dummy(new Vector2(0, 10));

        ImGui.Text($"Battery Percentage: {SystemPower.ChargePercentage}%");
        ImGui.Text($"Limetime (sec): {SystemPower.LifetimeSeconds}");
        ImGui.Text($"Charging: {SystemPower.IsCharging}");
        ImGui.Text($"Has Battery: {SystemPower.HasBattery}");
#endif
    }
    
}